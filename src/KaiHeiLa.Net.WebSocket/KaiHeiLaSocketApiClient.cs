using System.Text;
#if DEBUG_PACKETS
using System.Text.Encodings.Web;
#endif
using System.Text.Json;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.API.Rest;
using KaiHeiLa.Net.Queue;
using KaiHeiLa.Net.Rest;
using KaiHeiLa.Net.WebSockets;
using KaiHeiLa.WebSocket;

namespace KaiHeiLa.API;

internal class KaiHeiLaSocketApiClient : KaiHeiLaRestApiClient
{
    public event Func<GatewaySocketFrameType, Task> SentGatewayMessage
    {
        add => _sentGatewayMessageEvent.Add(value);
        remove => _sentGatewayMessageEvent.Remove(value);
    }
    private readonly AsyncEvent<Func<GatewaySocketFrameType, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<GatewaySocketFrameType, Task>>();

    public event Func<GatewaySocketFrameType, int?, object, Task> ReceivedGatewayEvent
    {
        add => _receivedGatewayEvent.Add(value);
        remove => _receivedGatewayEvent.Remove(value);
    }
    private readonly AsyncEvent<Func<GatewaySocketFrameType, int?, object, Task>> _receivedGatewayEvent = new();
    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
    
    private readonly bool _isExplicitUrl;
    private CancellationTokenSource _connectCancelToken;
    private string _gatewayUrl;
    private Guid? _sessionId;
    private int _lastSeq;
    public ConnectionState ConnectionState { get; private set; }
    internal IWebSocketClient WebSocketClient { get; }
    
    public KaiHeiLaSocketApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider, string userAgent, string acceptLanguage,
            string url = null, RetryMode defaultRetryMode = RetryMode.AlwaysRetry, JsonSerializerOptions serializerOptions = null,
            Func<IRateLimitInfo, Task> defaultRatelimitCallback = null)
        : base(restClientProvider, userAgent, acceptLanguage, defaultRetryMode, serializerOptions, defaultRatelimitCallback)
    {
        _gatewayUrl = url;
        if (url != null)
            _isExplicitUrl = true;
        
        WebSocketClient = webSocketProvider();
        WebSocketClient.TextMessage += OnTextMessage;
        WebSocketClient.BinaryMessage += OnBinaryMessage;
        WebSocketClient.Closed += async ex =>
        {
#if DEBUG_PACKETS
            Console.WriteLine(ex);
#endif

            await DisconnectAsync().ConfigureAwait(false);
            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        };
    }

    private async Task OnBinaryMessage(byte[] data, int index, int count)
    {
        await using var decompressed = new MemoryStream();
        using (var compressedStream = new MemoryStream(data))
        await using (var compressed = new InflaterInputStream(compressedStream))
        {
            await compressed.CopyToAsync(decompressed);
            decompressed.Position = 0;
        }
        
        GatewaySocketFrame gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(decompressed, _serializerOptions);
        if (gatewaySocketFrame is not null)
        {
#if DEBUG_PACKETS
            Console.WriteLine($"<- [{gatewaySocketFrame.Type}] : #{gatewaySocketFrame.Sequence} \n{JsonSerializer.Serialize(gatewaySocketFrame.Payload, new JsonSerializerOptions {WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping})}".TrimEnd('\n'));
#endif
            await _receivedGatewayEvent.InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, gatewaySocketFrame.Payload).ConfigureAwait(false);
        }
    }

    private async Task OnTextMessage(string message)
    {
        GatewaySocketFrame gatewaySocketFrame = JsonSerializer.Deserialize<GatewaySocketFrame>(message, _serializerOptions);
        if (gatewaySocketFrame is not null)
        {
#if DEBUG_PACKETS
            Console.WriteLine($"<- [{gatewaySocketFrame.Type}] : #{gatewaySocketFrame.Sequence} \n{JsonSerializer.Serialize(gatewaySocketFrame.Payload, new JsonSerializerOptions {WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping})}".TrimEnd('\n'));
#endif
            await _receivedGatewayEvent.InvokeAsync(gatewaySocketFrame.Type, gatewaySocketFrame.Sequence, gatewaySocketFrame.Payload).ConfigureAwait(false);
        }
    }

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _connectCancelToken?.Dispose();
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
        finally { _stateLock.Release(); }
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
            _connectCancelToken?.Dispose();
            _connectCancelToken = new CancellationTokenSource();
            WebSocketClient?.SetCancelToken(_connectCancelToken.Token);

            if (!_isExplicitUrl)
            {
                GetBotGatewayResponse botGatewayResponse = await GetBotGatewayAsync().ConfigureAwait(false);
                _gatewayUrl = $"{botGatewayResponse.Url}{(_sessionId is null ? string.Empty : $"&resume=1&sn={_lastSeq}&session_id={_sessionId}")}";
            }

#if DEBUG_PACKETS
            Console.WriteLine("Connecting to gateway: " + _gatewayUrl);
#endif

            await WebSocketClient!.ConnectAsync(_gatewayUrl).ConfigureAwait(false);
            ConnectionState = ConnectionState.Connected;
        }
        catch (Exception)
        {
            if (!_isExplicitUrl)
                _gatewayUrl = null;
            await DisconnectInternalAsync().ConfigureAwait(false);
            throw;
        }
    }
    
    public async Task DisconnectAsync(Exception ex = null)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync(ex).ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }

    internal override async Task DisconnectInternalAsync(Exception ex = null)
    {
        if (WebSocketClient == null)
            throw new NotSupportedException("This client is not configured with WebSocket support.");

        if (ConnectionState == ConnectionState.Disconnected) return;
        ConnectionState = ConnectionState.Disconnecting;
        
        try { _connectCancelToken?.Cancel(false); }
        catch
        {
            // ignored
        }

        if (ex is GatewayReconnectException)
            await WebSocketClient.DisconnectAsync(4000).ConfigureAwait(false);
        else
            await WebSocketClient.DisconnectAsync().ConfigureAwait(false);

        ConnectionState = ConnectionState.Disconnected;
    }
    
    public async Task SendHeartbeatAsync(int lastSeq, RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        await SendGatewayAsync(GatewaySocketFrameType.Ping, sequence: lastSeq, options: options).ConfigureAwait(false);
    }
    
    public async Task SendResumeAsync(int lastSeq, RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        await SendGatewayAsync(GatewaySocketFrameType.Resume, sequence: lastSeq, options: options).ConfigureAwait(false);
    }
    
    public Task SendGatewayAsync(GatewaySocketFrameType gatewaySocketFrameType, object payload = null, int? sequence = null, RequestOptions options = null)
        => SendGatewayInternalAsync(gatewaySocketFrameType, options, payload, sequence);
    private async Task SendGatewayInternalAsync(GatewaySocketFrameType gatewaySocketFrameType, RequestOptions options, object payload = null, int? sequence = null)
    {
        CheckState();

        payload = new GatewaySocketFrame { Type = gatewaySocketFrameType, Payload = payload, Sequence = sequence };
        byte[] bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
        
        options.IsGatewayBucket = true;
        if (options.BucketId == null)
            options.BucketId = GatewayBucket.Get(GatewayBucketType.Unbucketed).Id;
        await RequestQueue.SendAsync(new WebSocketRequest(WebSocketClient, bytes, true, gatewaySocketFrameType == GatewaySocketFrameType.Ping, options)).ConfigureAwait(false);
        await _sentGatewayMessageEvent.InvokeAsync(gatewaySocketFrameType).ConfigureAwait(false);
        
#if DEBUG_PACKETS
        Console.WriteLine($"-> [{gatewaySocketFrameType}] : #{sequence} \n{JsonSerializer.Serialize(payload, new JsonSerializerOptions {WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping})}".TrimEnd('\n'));
#endif
    }
}