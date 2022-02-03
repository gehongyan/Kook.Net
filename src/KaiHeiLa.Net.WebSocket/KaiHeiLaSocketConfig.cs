using KaiHeiLa.Net.WebSockets;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public class KaiHeiLaSocketConfig : KaiHeiLaRestConfig
{
    /// <summary>
    ///    Returns the encoding gateway should use.
    /// </summary>
    public const string GatewayEncoding = "json";

    /// <summary>
    ///     Gets or sets the WebSocket host to connect to. If <c>null</c>, the client will use the
    ///     /gateway endpoint.
    /// </summary>
    public string GatewayHost { get; set; } = null;
    
    /// <summary>
    ///     超时毫秒
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;
    
    /// <summary>
    ///     心跳间隔毫秒
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 30000;
    
    /// <summary>
    ///     处理程序超时毫秒
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;
    
    public int LargeThreshold { get; set; } = 250;

    /// <summary>
    ///     Gets or sets the provider used to generate new WebSocket connections.
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }
    
    
    public KaiHeiLaSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
    }
}