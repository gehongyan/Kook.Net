using Kook.Gateway;
using Kook.Net.Udp;
using Kook.Net.WebSockets;
using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Represents a configuration class for <see cref="KookSocketClient"/>.
/// </summary>
/// <remarks>
///     This configuration, based on <see cref="KookRestConfig"/>, helps determine several key configurations the
///     socket client depend on. For instance, message cache and connection timeout.
/// </remarks>
/// <example>
///     The following config enables the message cache and configures the client to always download user upon guild
///     availability.
///     <code language="cs">
///     var config = new KookSocketConfig
///     {
///         AlwaysDownloadUsers = true,
///         MessageCacheSize = 100
///     };
///     var client = new KookSocketClient(config);
///     </code>
/// </example>
public class KookSocketConfig : KookGatewayConfig
{
    /// <summary>
    ///     Gets or sets the WebSocket host to connect to. If <c>null</c>, the client will use the
    ///     /gateway endpoint.
    /// </summary>
    public string GatewayHost { get; set; } = null;

    /// <summary>
    ///     Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;

    /// <summary>
    ///     Gets the heartbeat interval of WebSocket connection in milliseconds.
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 30000;

    /// <summary>
    ///     Gets the RTCP interval of RTP connection in milliseconds.
    /// </summary>
    public const int RtcpIntervalMilliseconds = 5000;

    /// <summary>
    ///     Gets or sets the provider used to generate new WebSocket connections.
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookSocketConfig"/> class.
    /// </summary>
    public KookSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
        UdpSocketProvider = DefaultUdpSocketProvider.Instance;
    }

    internal KookSocketConfig Clone() => MemberwiseClone() as KookSocketConfig;
}
