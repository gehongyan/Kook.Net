using Kook.Net.Queue;
using Kook.Net.Queue.SynchronousImmediate;
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
    public string? GatewayHost { get; set; }

    /// <summary>
    ///     Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;

    /// <summary>
    ///     Gets the heartbeat interval of WebSocket connection in milliseconds.
    /// </summary>
    public int HeartbeatIntervalMilliseconds { get; internal set; } = 30000;

    /// <summary>
    ///     Gets the RTCP interval of RTP connection in milliseconds.
    /// </summary>
    public const int RtcpIntervalMilliseconds = 5000;

    /// <summary>
    ///     Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged.
    ///     Setting this property to <c>null</c>disables this check.
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;

    /// <summary>
    ///     Gets or sets the threshold quantity considered as joining a small number of guilds.
    /// </summary>
    public uint SmallNumberOfGuildsThreshold { get; set; } = 5;

    /// <summary>
    ///     Get or set the threshold quantity considered as joining a large number of guilds.
    /// </summary>
    public uint LargeNumberOfGuildsThreshold { get; set; } = 50;

    /// <summary>
    ///     Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero
    ///     disables the message cache entirely.
    /// </summary>
    public int MessageCacheSize { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the provider used to generate new WebSocket connections.
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }

    /// <summary>
    ///     Gets or sets the provider used to generate new UDP sockets.
    /// </summary>
    public UdpSocketProvider UdpSocketProvider { get; set; }

    /// <summary>
    ///     Gets or sets the startup mode of the socket client.
    /// </summary>
    public StartupCacheFetchMode StartupCacheFetchMode { get; set; } = StartupCacheFetchMode.Auto;

    /// <summary>
    ///     Gets or sets whether or not all users should be downloaded as guilds come available.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         Setting this property to <c>true</c> will cause the client to download all users
    ///         for all guilds upon startup.
    ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
    ///         traffic. If you are experiencing issues, try setting this to <c>false</c> and manually call
    ///         <see cref="KookSocketClient.DownloadUsersAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadUsers { get; set; } = false;

    /// <summary>
    ///     Gets or sets whether or not all voice states should be downloaded as guilds come available.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         Setting this property to <c>true</c> will cause the client to download all voice states
    ///         for all guilds upon startup.
    ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
    ///         traffic. If you are experiencing issues, try setting this to <c>false</c> and manually call
    ///         <see cref="KookSocketClient.DownloadVoiceStatesAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadVoiceStates { get; set; } = false;

    /// <summary>
    ///     Gets or sets whether or not all boost subscriptions should be downloaded as guilds come available.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         Setting this property to <c>true</c> will cause the client to download all boost subscriptions
    ///         for all guilds upon startup and when <see cref="BaseSocketClient.GuildUpdated"/> is triggered with
    ///         changes occurring to <see cref="SocketGuild.BoostSubscriptionCount"/>.
    ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
    ///         traffic. If you are experiencing issues, try setting this to <c>false</c> and manually call
    ///         <see cref="KookSocketClient.DownloadBoostSubscriptionsAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadBoostSubscriptions { get; set; } = false;

    /// <summary>
    ///     Gets or sets the maximum number of times to retry fetching joined guild data.
    /// </summary>
    /// <remarks>
    ///     Due to the KOOK API cannot return the newly joined guilds immediately well, this property is used to
    ///     control the maximum number of times to retry fetching joined guild data. Each retry will be delayed
    ///     by <see cref="JoinedGuildDataFetchingRetryDelay"/> milliseconds. Set to 0 or negative value to disable
    ///     retrying.
    /// </remarks>
    public int MaxJoinedGuildDataFetchingRetryTimes { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the delay in milliseconds between each retry of fetching joined guild data.
    /// </summary>
    /// <exception cref="System.ArgumentException">Value must be at least 0.</exception>
    public int JoinedGuildDataFetchingRetryDelay
    {
        get => _joinedGuildDataFetchingRetryDelay;
        set
        {
            Preconditions.AtLeast(value, 0, nameof(JoinedGuildDataFetchingRetryDelay));
            _joinedGuildDataFetchingRetryDelay = value;
        }
    }

    private int _joinedGuildDataFetchingRetryDelay = 500;

    /// <summary>
    ///     Gets or sets whether to update guild role positions via API when <see cref="BaseSocketClient.GuildUpdated"/> fires.
    /// </summary>
    public bool AutoUpdateRolePositions { get; set; } = false;

    /// <summary>
    ///     Gets or sets whether to update guild channels via API when gateway publishes sort_channel events.
    /// </summary>
    public bool AutoUpdateChannelPositions { get; set; } = false;

    /// <summary>
    ///     Gets or sets the provider used to generate new message queues.
    /// </summary>
    public MessageQueueProvider MessageQueueProvider { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookSocketConfig"/> class.
    /// </summary>
    public KookSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
        UdpSocketProvider = DefaultUdpSocketProvider.Instance;
        MessageQueueProvider = SynchronousImmediateMessageQueueProvider.Instance;
    }

    internal KookSocketConfig Clone() => (KookSocketConfig)MemberwiseClone();
}
