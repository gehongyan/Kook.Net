using Kook.API;
using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Represents an abstract base class for WebSocket-based clients.
/// </summary>
public abstract partial class BaseSocketClient : BaseKookClient, IKookClient
{
    /// <summary>
    ///     Gets the configuration used by this client.
    /// </summary>
    protected readonly KookSocketConfig _baseConfig;

    /// <summary>
    ///     Gets the estimated round-trip latency, in milliseconds, to the gateway server.
    /// </summary>
    /// <returns>
    ///     An int that represents the round-trip latency to the WebSocket server. Please
    ///     note that this value does not represent a "true" latency for operations such as sending a message.
    /// </returns>
    public abstract int Latency { get; protected set; }

    /// <summary>
    ///     Provides access to a REST-only client with a shared state from this client.
    /// </summary>
    public abstract KookSocketRestClient Rest { get; }

    internal new KookSocketApiClient ApiClient => base.ApiClient as KookSocketApiClient;

    /// <summary>
    ///     Gets the current logged-in user.
    /// </summary>
    public new virtual SocketSelfUser CurrentUser
    {
        get => base.CurrentUser as SocketSelfUser;
        protected set => base.CurrentUser = value;
    }

    /// <summary>
    ///     Gets a collection of guilds that the user is currently in.
    /// </summary>
    /// <returns>
    ///     A read-only collection of guilds that the current user is in.
    /// </returns>
    public abstract IReadOnlyCollection<SocketGuild> Guilds { get; }

    internal BaseSocketClient(KookSocketConfig config, KookRestApiClient client)
        : base(config, client) => _baseConfig = config;

    /// <summary>
    ///     Gets a generic user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <remarks>
    ///     This method gets the user present in the WebSocket cache with the given condition.
    ///     <note type="warning">
    ///         Sometimes a user may return <c>null</c> due to Kook not sending offline users in large guilds
    ///         (i.e. guild with 100+ members) actively. To download users on startup and to see more information
    ///         about this subject, see <see cref="Kook.WebSocket.KookSocketConfig.AlwaysDownloadUsers" />.
    ///     </note>
    ///     <note>
    ///         This method does not attempt to fetch users that the logged-in user does not have access to (i.e.
    ///         users who don't share mutual guild(s) with the current user). If you wish to get a user that you do
    ///         not have access to, consider using the REST implementation of
    ///         <see cref="KookRestClient.GetUserAsync(System.UInt64,Kook.RequestOptions)" />.
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A generic WebSocket-based user; <c>null</c> when the user cannot be found.
    /// </returns>
    public abstract SocketUser GetUser(ulong id);

    /// <summary>
    ///     Gets a user.
    /// </summary>
    /// <remarks>
    ///     This method gets the user present in the WebSocket cache with the given condition.
    ///     <note type="warning">
    ///         Sometimes a user may return <c>null</c> due to Kook not sending offline users in large guilds
    ///         (i.e. guild with 100+ members) actively. To download users on startup and to see more information
    ///         about this subject, see <see cref="Kook.WebSocket.KookSocketConfig.AlwaysDownloadUsers" />.
    ///     </note>
    ///     <note>
    ///         This method does not attempt to fetch users that the logged-in user does not have access to (i.e.
    ///         users who don't share mutual guild(s) with the current user). If you wish to get a user that you do
    ///         not have access to, consider using the REST implementation of
    ///         <see cref="KookRestClient.GetUserAsync(System.UInt64,Kook.RequestOptions)" />.
    ///     </note>
    /// </remarks>
    /// <param name="username">The name of the user.</param>
    /// <param name="identifyNumber">The identify value of the user.</param>
    /// <returns>
    ///     A generic WebSocket-based user; <c>null</c> when the user cannot be found.
    /// </returns>
    public abstract SocketUser GetUser(string username, string identifyNumber);

    /// <summary>
    ///     Gets a channel.
    /// </summary>
    /// <param name="id">The identifier of the channel.</param>
    /// <returns>
    ///     A generic WebSocket-based channel object (voice, text, category, etc.) associated with the identifier;
    ///     <c>null</c> when the channel cannot be found.
    /// </returns>
    public abstract SocketChannel GetChannel(ulong id);

    /// <summary>
    ///     Gets a channel.
    /// </summary>
    /// <param name="chatCode">The chat code of the direct-message channel.</param>
    /// <returns>
    ///     A generic WebSocket-based channel object (voice, text, category, etc.) associated with the identifier;
    ///     <c>null</c> when the channel cannot be found.
    /// </returns>
    public abstract SocketDMChannel GetDMChannel(Guid chatCode);

    /// <summary>
    ///     Gets a channel.
    /// </summary>
    /// <param name="userId">The user identifier of the direct-message channel.</param>
    /// <returns>
    ///     A generic WebSocket-based channel object (voice, text, category, etc.) associated with the identifier;
    ///     <c>null</c> when the channel cannot be found.
    /// </returns>
    public abstract SocketDMChannel GetDMChannel(ulong userId);

    /// <summary>
    ///     Gets a guild.
    /// </summary>
    /// <param name="id">The guild identifier.</param>
    /// <returns>
    ///     A WebSocket-based guild associated with the identifier; <c>null</c> when the guild cannot be
    ///     found.
    /// </returns>
    public abstract SocketGuild GetGuild(ulong id);

    /// <summary>
    ///     Starts the WebSocket connection.
    /// </summary>
    /// <returns> A task that represents the asynchronous start operation. </returns>
    public abstract Task StartAsync();

    /// <summary>
    ///     Stops the WebSocket connection.
    /// </summary>
    /// <returns> A task that represents the asynchronous stop operation. </returns>
    public abstract Task StopAsync();

    /// <summary>
    ///     Attempts to download users into the user cache for the selected guilds.
    /// </summary>
    /// <param name="guilds">The guilds to download the members from.</param>
    /// <param name="options"> The options to be used when sending the request. </param>
    /// <returns>
    ///     A task that represents the asynchronous download operation.
    /// </returns>
    public abstract Task DownloadUsersAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null);

    /// <summary>
    ///     Downloads all voice states for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the voice states for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public abstract Task DownloadVoiceStatesAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null);

    /// <summary>
    ///     Downloads all boost subscriptions for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the boost subscriptions for. If <c>null</c>, all available guilds will be downloaded.
    ///     To download all boost subscriptions, the current user must has the
    ///     <see cref="GuildPermission.ManageGuild"/> permission.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public abstract Task DownloadBoostSubscriptionsAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null);

    /// <inheritdoc />
    Task<IChannel> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IChannel>(GetChannel(id));

    /// <inheritdoc />
    Task<IDMChannel> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions options)
        => Task.FromResult<IDMChannel>(GetDMChannel(chatCode));

    /// <inheritdoc />
    Task<IGuild> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuild>(GetGuild(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

    /// <inheritdoc />
    async Task<IUser> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        SocketUser user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly) return user;

        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    Task<IUser> IKookClient.GetUserAsync(string username, string identifyNumber, RequestOptions options)
        => Task.FromResult<IUser>(GetUser(username, identifyNumber));
}
