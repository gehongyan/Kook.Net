using System.IO.Compression;
using System.Text;
#if DEBUG_PACKETS
using System.Diagnostics;
using System.Text.Encodings.Web;
#endif
using System.Text.Json;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Net.Queue;
using Kook.Net.Rest;
using Kook.Net.WebSockets;
using Kook.WebSocket;

namespace Kook.API;

internal class KookSocketApiClient : KookRestApiClient
{
    public event Func<GatewaySocketFrameType, Task> SentGatewayMessage
    {
        add => _sentGatewayMessageEvent.Add(value);
        remove => _sentGatewayMessageEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GatewaySocketFrameType, Task>> _sentGatewayMessageEvent = new();

    public event Func<GatewaySocketFrameType, int?, JsonElement, Task> ReceivedGatewayEvent
    {
        add => _receivedGatewayEvent.Add(value);
        remove => _receivedGatewayEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GatewaySocketFrameType, int?, JsonElement, Task>> _receivedGatewayEvent = new();

    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new();

    private readonly bool _isExplicitUrl;
    private CancellationTokenSource? _connectCancellationToken;
    private string? _gatewayUrl;
    private Guid? _sessionId;
    private int _lastSeq;

#if DEBUG_PACKETS
    private readonly JsonSerializerOptions _debugJsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
#endif

    public ConnectionState ConnectionState { get; private set; }
    internal IWebSocketClient WebSocketClient { get; }

    public KookSocketApiClient(RestClientProvider restClientProvider,
        WebSocketProvider webSocketProvider,
        string userAgent, string acceptLanguage, string? url = null,
        RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions? serializerOptions = null,
        Func<IRateLimitInfo, Task>? defaultRatelimitCallback = null)
        : base(restClientProvider, userAgent, acceptLanguage,
            defaultRetryMode, serializerOptions, defaultRatelimitCallback)
    {
        _gatewayUrl = url;
        if (url != null)
            _isExplicitUrl = true;
        WebSocketClient = webSocketProvider();
        WebSocketClient.SetKeepAliveInterval(TimeSpan.Zero);
        WebSocketClient.TextMessage += OnTextMessage;
        WebSocketClient.BinaryMessage += OnBinaryMessage;
        WebSocketClient.Closed += async ex =>
        {
#if DEBUG_PACKETS
            Debug.WriteLine(ex);
#endif
            await DisconnectAsync().ConfigureAwait(false);
            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        };
    }

    private async Task OnBinaryMessage(byte[] data, int index, int count)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        await using MemoryStream decompressed = new();
#else
        using MemoryStream decompressed = new();
#endif
        using MemoryStream compressed = data[0] == 0x78
            ? new MemoryStream(data, index + 2, count - 2)
            : new MemoryStream(data, index, count);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        await using DeflateStream decompressor = new(compressed, CompressionMode.Decompress);
#else
        using DeflateStream decompressor = new(compressed, CompressionMode.Decompress);
#endif
        await decompressor.CopyToAsync(decompressed);
        decompressed.Position = 0;

        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer
            .Deserialize<GatewaySocketFrame>(decompressed, _serializerOptions);
        if (gatewaySocketFrame is not null)
        {
#if DEBUG_PACKETS
            string raw = Encoding.Default.GetString(decompressed.ToArray()).TrimEnd('\n');
            string parsed = JsonSerializer
                .Serialize(gatewaySocketFrame.Payload, _debugJsonSerializerOptions)
                .TrimEnd('\n');
            Debug.WriteLine($"""
                [{DateTimeOffset.Now:HH:mm:ss}] <- [{gatewaySocketFrame.Type}] : #{gatewaySocketFrame.Sequence}
                [Raw] {raw}
                [Parsed] {parsed}
                """);
#endif
            JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;
            await _receivedGatewayEvent
                .InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
                .ConfigureAwait(false);
        }
    }

    private async Task OnTextMessage(string message)
    {
        GatewaySocketFrame? gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(message, _serializerOptions);
        if (gatewaySocketFrame is null)
            return;
#if DEBUG_PACKETS
        string parsed = JsonSerializer
            .Serialize(gatewaySocketFrame.Payload, _debugJsonSerializerOptions)
            .TrimEnd('\n');
        Debug.WriteLine($"""
            [{DateTimeOffset.Now:HH:mm:ss}] <- [{gatewaySocketFrame.Type}] : #{gatewaySocketFrame.Sequence}
            [Raw] {message}
            [Parsed] {parsed}
            """);
#endif
        JsonElement payloadElement = gatewaySocketFrame.Payload as JsonElement? ?? EmptyJsonElement;
        await _receivedGatewayEvent
            .InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, payloadElement)
            .ConfigureAwait(false);
    }

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _connectCancellationToken?.Dispose();
                WebSocketClient?.Dispose();
            }

            _isDisposed = true;
        }

        base.Dispose(disposing);
    }

    public async Task ConnectAsync(Guid? sessionId, int lastSeq)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _sessionId = sessionId;
            _lastSeq = lastSeq;
            await ConnectInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _stateLock.Release();
        }
    }

    internal override async Task ConnectInternalAsync()
    {
        if (LoginState != LoginState.LoggedIn)
            throw new InvalidOperationException("The client must be logged in before connecting.");
        if (WebSocketClient == null)
            throw new NotSupportedException("This client is not configured with WebSocket support.");
        RequestQueue.ClearGatewayBuckets();
        ConnectionState = ConnectionState.Connecting;
        try
        {
            _connectCancellationToken?.Dispose();
            _connectCancellationToken = new CancellationTokenSource();
            WebSocketClient.SetCancellationToken(_connectCancellationToken.Token);

            if (!_isExplicitUrl || _gatewayUrl == null)
            {
                GetBotGatewayResponse botGatewayResponse = await GetBotGatewayAsync().ConfigureAwait(false);
                string resumeQuery = _sessionId is not null
                    ? $"&resume=1&sn={_lastSeq}&session_id={_sessionId}"
                    : string.Empty;
                _gatewayUrl = $"{botGatewayResponse.Url}{resumeQuery}";
            }
#if DEBUG_PACKETS
            Debug.WriteLine("Connecting to gateway: " + _gatewayUrl);
#endif
            await WebSocketClient.ConnectAsync(_gatewayUrl).ConfigureAwait(false);
            ConnectionState = ConnectionState.Connected;
        }
        catch (Exception)
        {
            if (!_isExplicitUrl) _gatewayUrl = null;

            await DisconnectInternalAsync().ConfigureAwait(false);
            throw;
        }
    }

    public async Task DisconnectAsync(Exception? ex = null)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync(ex).ConfigureAwait(false);
        }
        finally
        {
            _stateLock.Release();
        }
    }

    internal override async Task DisconnectInternalAsync(Exception? ex = null)
    {
        if (WebSocketClient == null)
            throw new NotSupportedException("This client is not configured with WebSocket support.");
        if (ConnectionState == ConnectionState.Disconnected)
            return;
        ConnectionState = ConnectionState.Disconnecting;

        if (ex is GatewayReconnectException)
            await WebSocketClient.DisconnectAsync(4000).ConfigureAwait(false);
        else
            await WebSocketClient.DisconnectAsync().ConfigureAwait(false);

        try
        {
            _connectCancellationToken?.Cancel(false);
        }
        catch
        {
            // ignored
        }

        ConnectionState = ConnectionState.Disconnected;
    }

    public async Task SendHeartbeatAsync(int lastSeq, RequestOptions? options = null)
    {
        RequestOptions requestOptions = RequestOptions.CreateOrClone(options);
        await SendGatewayAsync(GatewaySocketFrameType.Ping, null, lastSeq, requestOptions)
            .ConfigureAwait(false);
    }

    public async Task SendResumeAsync(int lastSeq, RequestOptions? options = null)
    {
        RequestOptions requestOptions = RequestOptions.CreateOrClone(options);
        await SendGatewayAsync(GatewaySocketFrameType.Resume, null, lastSeq, requestOptions)
            .ConfigureAwait(false);
    }

    public Task SendGatewayAsync(GatewaySocketFrameType gatewaySocketFrameType,
        object? payload, int? sequence, RequestOptions options) =>
        SendGatewayInternalAsync(gatewaySocketFrameType, payload, sequence, options);

    private async Task SendGatewayInternalAsync(GatewaySocketFrameType gatewaySocketFrameType,
        object? payload, int? sequence, RequestOptions options)
    {
        CheckState();
        payload = new GatewaySocketFrame
        {
            Type = gatewaySocketFrameType,
            Payload = payload,
            Sequence = sequence
        };
        string json = SerializeJson(payload);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        options.IsGatewayBucket = true;
        options.BucketId ??= GatewayBucket.Get(GatewayBucketType.Unbucketed).Id;
        bool ignoreLimit = gatewaySocketFrameType == GatewaySocketFrameType.Ping;
        await RequestQueue
            .SendAsync(new WebSocketRequest(WebSocketClient, bytes, true, ignoreLimit, options))
            .ConfigureAwait(false);
        await _sentGatewayMessageEvent.InvokeAsync(gatewaySocketFrameType).ConfigureAwait(false);

#if DEBUG_PACKETS
        string payloadString = JsonSerializer.Serialize(payload, _debugJsonSerializerOptions);
        Debug.WriteLine($"[{DateTimeOffset.Now:HH:mm:ss}] -> [{gatewaySocketFrameType}] : #{sequence} \n{payloadString}".TrimEnd('\n'));
#endif
    }
}
