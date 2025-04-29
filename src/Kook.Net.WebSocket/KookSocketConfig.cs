using Kook.Net.Queue;
using Kook.Net.Queue.SynchronousImmediate;
using Kook.Net.Udp;
using Kook.Net.WebSockets;
using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     表示一个用于 <see cref="Kook.WebSocket.KookSocketClient"/> 的配置类。
/// </summary>
/// <remarks>
///     此配置基于 <see cref="Kook.Rest.KookRestConfig"/>，在与 REST 有关的配置的基础上，定义了有关网关的配置。
/// </remarks>
/// <example>
///     以下代码启用了消息缓存，并配置客户端在服务器可用时始终下载用户。
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
    ///     获取网关使用的数据格式。
    /// </summary>
    public const string GatewayEncoding = "json";

    /// <summary>
    ///     获取或设置要连接的网关地址。如果为 <c>null</c>，则客户端将会通过 API 请求获取网关地址。
    /// </summary>
    public string? GatewayHost { get; set; }

    /// <summary>
    ///     获取或设置连接到网关时的超时时间间隔（毫秒）。
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;

    /// <summary>
    ///     获取网关发送心跳包的时间间隔（毫秒）。
    /// </summary>
    public int HeartbeatIntervalMilliseconds { get; internal set; } = 30000;

    /// <summary>
    ///     获取语音客户端 RTP 连接中发送 RTCP 数据报的时间间隔（毫秒）。
    /// </summary>
    public const int RtcpIntervalMilliseconds = 5000;

    /// <summary>
    ///     获取或设置阻塞网关线程的事件处理程序的超时时间间隔（毫秒），超过此时间间隔的阻塞网关线程的事件处理程序会被日志记录警告。将此属性设置为 <c>null</c> 将禁用此检查。
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;

    /// <summary>
    ///     获取或设置被视为加入少量服务器的阈值数量。
    /// </summary>
    /// <seealso cref="Kook.WebSocket.StartupCacheFetchMode.Auto"/>
    public uint SmallNumberOfGuildsThreshold { get; set; } = 5;

    /// <summary>
    ///     获取或设置被视为加入大量服务器的阈值数量。
    /// </summary>
    /// <seealso cref="Kook.WebSocket.StartupCacheFetchMode.Auto"/>
    public uint LargeNumberOfGuildsThreshold { get; set; } = 50;

    /// <summary>
    ///     获取或设置应在缓存中保留的每个频道的消息数量。将此属性设置为零将完全禁用消息缓存。
    /// </summary>
    public int MessageCacheSize { get; set; } = 10;

    /// <summary>
    ///     获取或设置用于创建 WebSocket 客户端的委托。
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }

    /// <summary>
    ///     获取或设置用于创建 UDP 客户端的委托。
    /// </summary>
    public UdpSocketProvider UdpSocketProvider { get; set; }

    /// <summary>
    ///     获取或设置在启动时缓存获取模式。
    /// </summary>
    /// <remarks>
    ///     此属性用于指定客户端在启动时如何缓存基础数据，并影响 <see cref="Kook.WebSocket.KookSocketClient.Ready"/> 事件的引发时机。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    /// </remarks>
    public StartupCacheFetchMode StartupCacheFetchMode { get; set; } = StartupCacheFetchMode.Auto;

    /// <summary>
    ///     获取或设置音频客户端被视为空闲的超时时间间隔（毫秒）。
    /// </summary>
    public int AudioClientIdleTimeout { get; set; } = 15000;

    /// <summary>
    ///     获取或设置是否在服务器可用时始终下载所有用户。
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         对于大型服务器，启用此选项可能会导致性能问题。调用
    ///         <see cref="Kook.WebSocket.KookSocketClient.DownloadUsersAsync(System.Collections.Generic.IEnumerable{Kook.IGuild},Kook.RequestOptions)"/>
    ///         可以按需下载服务器用户列表。
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadUsers { get; set; } = false;

    /// <summary>
    ///     获取或设置是否在服务器可用时始终下载所有语音状态。
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         对于大型服务器，启用此选项可能会导致性能问题。调用
    ///         <see cref="Kook.WebSocket.KookSocketClient.DownloadVoiceStatesAsync(System.Collections.Generic.IEnumerable{Kook.IGuild},Kook.RequestOptions)"/>
    ///         可以按需下载服务器语音状态。
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadVoiceStates { get; set; } = false;

    /// <summary>
    ///     获取或设置是否在服务器可用时始终下载所有服务器的所有服务器助力信息。
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         当此属性为 <c>true</c> 时，客户端将在启动时下载所有服务器的所有服务器助力信息，并在引发
    ///         <see cref="Kook.WebSocket.BaseSocketClient.GuildUpdated"/> 事件时，当 <see cref="SocketGuild.BoostSubscriptionCount"/>
    ///         发生更改时，也会重新下载所有服务器的所有服务器助力信息。 <br />
    ///         对于大型服务器，启用此选项可能会导致性能问题。调用
    ///         <see cref="Kook.WebSocket.KookSocketClient.DownloadBoostSubscriptionsAsync(System.Collections.Generic.IEnumerable{Kook.IGuild},Kook.RequestOptions)"/>
    ///         可以按需下载服务器的所有服务器助力信息。
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadBoostSubscriptions { get; set; } = false;

    /// <summary>
    ///     获取或设置获取新加入服务器数据的最大重试次数。
    /// </summary>
    /// <remarks>
    ///     KOOK API 无法立即返回刚刚新加入的服务器数据，因此此属性用于控制获取加入的服务器数据的最大重试次数。
    ///     每次重试前都会等待 <see cref="Kook.WebSocket.KookSocketConfig.JoinedGuildDataFetchingRetryDelay"/>
    ///     毫秒。将当前属性设置为 0 或负值以禁用重试。
    /// </remarks>
    public int MaxJoinedGuildDataFetchingRetryTimes { get; set; } = 10;

    /// <summary>
    ///     获取或设置获取新加入服务器数据每次重试之前所等待的时间间隔（毫秒）。
    /// </summary>
    /// <exception cref="System.ArgumentException"> 时间间隔不能小于 <c>0</c>。 </exception>
    /// <seealso cref="Kook.WebSocket.KookSocketConfig.MaxJoinedGuildDataFetchingRetryTimes"/>
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
    ///     获取或设置是否在引发 <see cref="Kook.WebSocket.BaseSocketClient.GuildUpdated"/> 事件时通过 API 更新服务器角色位置。
    /// </summary>
    public bool AutoUpdateRolePositions { get; set; } = false;

    /// <summary>
    ///     获取或设置是否在网关发布 <c>sort_channel</c> 事件时通过 API 更新服务器频道。
    /// </summary>
    public bool AutoUpdateChannelPositions { get; set; } = false;

    /// <summary>
    ///     获取或设置用于创建消息队列的委托。
    /// </summary>
    public MessageQueueProvider MessageQueueProvider { get; set; }

    /// <summary>
    ///     获取或设置是否不在接收到未知网关事件消息是输出警告。
    /// </summary>
    public bool SuppressUnknownDispatchWarnings { get; set; } = false;

    /// <summary>
    ///     初始化一个 <see cref="KookSocketConfig"/> 类的新实例。
    /// </summary>
    public KookSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
        UdpSocketProvider = DefaultUdpSocketProvider.Instance;
        MessageQueueProvider = SynchronousImmediateMessageQueueProvider.Instance;
    }

    internal KookSocketConfig Clone() => (KookSocketConfig)MemberwiseClone();
}
