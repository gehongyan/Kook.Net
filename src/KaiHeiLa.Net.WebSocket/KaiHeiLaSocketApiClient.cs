using System.Text;
using System.Text.Json;
using KaiHeiLa.Net.Queue;
using KaiHeiLa.Net.Rest;
using KaiHeiLa.Net.WebSockets;
using KaiHeiLa.WebSocket;

namespace KaiHeiLa.API;

internal class KaiHeiLaSocketApiClient : KaiHeiLaRestApiClient
{
    public event Func<SocketFrameType, Task> SentGatewayMessage
    {
        add => _sentGatewayMessageEvent.Add(value);
        remove => _sentGatewayMessageEvent.Remove(value);
    }
    private readonly AsyncEvent<Func<SocketFrameType, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<SocketFrameType, Task>>();

    public event Func<SocketFrameType, int?, object, Task> ReceivedGatewayEvent
    {
        add => _receivedGatewayEvent.Add(value);
        remove => _receivedGatewayEvent.Remove(value);
    }
    private readonly AsyncEvent<Func<SocketFrameType, int?, object, Task>> _receivedGatewayEvent = new();
    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
    
    private readonly bool _isExplicitUrl;
    private CancellationTokenSource _connectCancelToken;
    private string _gatewayUrl;
    private string _resumeQueryParams;
    public ConnectionState ConnectionState { get; private set; }
    internal IWebSocketClient WebSocketClient { get; }
    
    public KaiHeiLaSocketApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider, string userAgent,
            string url = null, RetryMode defaultRetryMode = RetryMode.AlwaysRetry, JsonSerializerOptions serializerOptions = null,
            Func<IRateLimitInfo, Task> defaultRatelimitCallback = null)
        : base(restClientProvider, userAgent, defaultRetryMode, serializerOptions, defaultRatelimitCallback)
    {
        _gatewayUrl = url;
        if (url != null)
            _isExplicitUrl = true;
        
        WebSocketClient = webSocketProvider();
        WebSocketClient.TextMessage += OnTextMessage;
        WebSocketClient.Closed += async ex =>
        {
#if DEBUG_PACKETS
                Console.WriteLine(ex);
#endif

            await DisconnectAsync().ConfigureAwait(false);
            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        };
        _resumeQueryParams = null;
    }
    
    private async Task OnTextMessage(string message)
    {
        SocketFrame socketFrame = JsonSerializer.Deserialize<SocketFrame>(message, SerializerOptions);
        if (socketFrame is not null)
        {
            await _receivedGatewayEvent.InvokeAsync(socketFrame.Type, socketFrame.Sequence, socketFrame.Payload).ConfigureAwait(false);
        }
    }

    public async Task StartAsync()
    {
        
    }

    public void SetResumeParams(int lastSeq, Guid sessionId)
    {
        _resumeQueryParams = $"&resume=1&sn={lastSeq}&session_id={sessionId}";
    }

    public void ClearResumeParams()
    {
        _resumeQueryParams = null;
    }
    
    public async Task ConnectAsync()
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
       try
        {
            await ConnectInternalAsync().ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }

    internal async Task ConnectInternalAsync()
    {
        if (LoginState != LoginState.LoggedIn)
            throw new InvalidOperationException("The client must be logged in before connecting.");
        if (WebSocketClient == null)
            throw new NotSupportedException("This client is not configured with WebSocket support.");
        
        ConnectionState = ConnectionState.Connecting;

        try
        {
            _connectCancelToken?.Dispose();
            _connectCancelToken = new CancellationTokenSource();
            WebSocketClient?.SetCancelToken(_connectCancelToken.Token);

            GetGatewayResponse gatewayResponse = await GetGatewayAsync().ConfigureAwait(false);
            _gatewayUrl = $"{gatewayResponse.Url}{_resumeQueryParams}";
            await WebSocketClient.ConnectAsync(_gatewayUrl).ConfigureAwait(false);
            ConnectionState = ConnectionState.Connected;
        }
        catch
        {
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

    internal async Task DisconnectInternalAsync(Exception ex = null)
    {
        if (WebSocketClient == null)
            throw new NotSupportedException("This client is not configured with WebSocket support.");

        if (ConnectionState == ConnectionState.Disconnected) return;
        ConnectionState = ConnectionState.Disconnecting;
        
        try { _connectCancelToken?.Cancel(false); }
        catch { }
        
        if (ex is GatewayReconnectException)
            await WebSocketClient.DisconnectAsync(4000).ConfigureAwait(false);
        else
            await WebSocketClient.DisconnectAsync().ConfigureAwait(false);

        ConnectionState = ConnectionState.Disconnected;
    }
    
    public async Task SendHeartbeatAsync(int lastSeq, RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        await SendGatewayAsync(SocketFrameType.Ping, lastSeq, options: options).ConfigureAwait(false);
    }
    
    public Task SendGatewayAsync(SocketFrameType socketFrameType, object payload = null, int? sequence = null, RequestOptions options = null)
        => SendGatewayInternalAsync(socketFrameType, options, payload, sequence);
    private async Task SendGatewayInternalAsync(SocketFrameType socketFrameType, RequestOptions options, object payload = null, int? sequence = null)
    {
        CheckState();
        
        byte[] bytes = null;
        payload = new SocketFrame { Type = socketFrameType, Payload = payload, Sequence = sequence };
        if (payload != null)
            bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
        
        options.IsGatewayBucket = true;
        if (options.BucketId == null)
            options.BucketId = GatewayBucket.Get(GatewayBucketType.Unbucketed).Id;
        await RequestQueue.SendAsync(new WebSocketRequest(WebSocketClient, bytes, true, socketFrameType == SocketFrameType.Ping, options)).ConfigureAwait(false);
        await _sentGatewayMessageEvent.InvokeAsync(socketFrameType).ConfigureAwait(false);
        
#if DEBUG_PACKETS
        Console.WriteLine($"-> {opCode}:\n{SerializeJson(payload)}");
#endif
    }
}