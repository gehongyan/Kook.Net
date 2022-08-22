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
public class KookSocketConfig : KookRestConfig
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
    ///     Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;
    
    /// <summary>
    ///     Gets the heartbeat interval of WebSocket connection in milliseconds.
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 30000;
    
    /// <summary>
    ///     Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged.
    ///     Setting this property to <c>null</c>disables this check.
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;
    
    /// <summary>
    ///     Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero
    ///     disables the message cache entirely.
    /// </summary>
    public int MessageCacheSize { get; set; } = 10;
    
    /// <summary>
    ///     Gets or sets the max number of users a guild may have for offline users to be included in the READY
    ///     packet. The maximum value allowed is 250.
    /// </summary>
    public int LargeThreshold { get; set; } = 250;

    /// <summary>
    ///     Gets or sets the provider used to generate new WebSocket connections.
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }

    /// <summary>
    ///     Gets or sets the provider used to generate new UDP sockets.
    /// </summary>
    public UdpSocketProvider UdpSocketProvider { get; set; }
    
    /// <summary>
    ///     Gets or sets whether or not all users should be downloaded as guilds come available.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
    ///         traffic. If you are experiencing issues, try setting this to <c>false</c> and manually call
    ///         <see cref="KookSocketClient.DownloadUsersAsync(IEnumerable{IGuild})"/> on the guilds you want.
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadUsers { get; set; } = false;
    
    /// <summary>
    ///     Gets or sets the maximum wait time in milliseconds between GUILD_AVAILABLE events before firing READY.
    ///     If zero, READY will fire as soon as it is received and all guilds will be unavailable.
    /// </summary>
    /// <remarks>
    ///     <para>This property is measured in milliseconds; negative values will throw an exception.</para>
    ///     <para>If a guild is not received before READY, it will be unavailable.</para>
    /// </remarks>
    /// <returns>
    ///     An int representing the maximum wait time in milliseconds between GUILD_AVAILABLE events
    ///     before firing READY.
    /// </returns>
    /// <exception cref="System.ArgumentException">Value must be at least 0.</exception>
    public int MaxWaitBetweenGuildAvailablesBeforeReady
    {
        get
        {
            return _maxWaitForGuildAvailable;
        }

        set
        {
            Preconditions.AtLeast(value, 0, nameof(MaxWaitBetweenGuildAvailablesBeforeReady));
            _maxWaitForGuildAvailable = value;
        }
    }

    private int _maxWaitForGuildAvailable = 10000;
    
    public KookSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
        UdpSocketProvider = DefaultUdpSocketProvider.Instance;
    }
    
    internal KookSocketConfig Clone() => MemberwiseClone() as KookSocketConfig;
}