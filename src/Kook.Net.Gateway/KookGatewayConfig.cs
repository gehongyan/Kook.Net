using Kook.Net.Udp;
using Kook.Rest;

namespace Kook.Gateway;

/// <summary>
///     Represents the configuration for Kook.Net gateway.
/// </summary>
public class KookGatewayConfig : KookRestConfig
{
    /// <summary>
    ///    Returns the encoding gateway should use.
    /// </summary>
    public const string GatewayEncoding = "json";

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
    ///     Gets or sets the provider used to generate new UDP sockets.
    /// </summary>
    public UdpSocketProvider UdpSocketProvider { get; set; }

    /// <summary>
    ///     Gets or sets whether or not all users should be downloaded as guilds come available.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         Setting this property to <c>true</c> will cause the client to download all users
    ///         for all guilds upon startup.
    ///         Please note that it can be difficult to fill the cache completely on large guilds depending on the
    ///         traffic. If you are experiencing issues, try setting this to <c>false</c> and manually call
    ///         <see cref="KookGatewayClient.DownloadUsersAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
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
    ///         <see cref="KookGatewayClient.DownloadVoiceStatesAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
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
    ///         <see cref="KookGatewayClient.DownloadBoostSubscriptionsAsync(IEnumerable{IGuild},RequestOptions)"/> on the guilds you want.
    ///     </note>
    /// </remarks>
    public bool AlwaysDownloadBoostSubscriptions { get; set; } = false;

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
        get => _maxWaitForGuildAvailable;

        set
        {
            Preconditions.AtLeast(value, 0, nameof(MaxWaitBetweenGuildAvailablesBeforeReady));
            _maxWaitForGuildAvailable = value;
        }
    }

    private int _maxWaitForGuildAvailable = 10000;

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

    internal KookGatewayConfig Clone() => MemberwiseClone() as KookGatewayConfig;
}
