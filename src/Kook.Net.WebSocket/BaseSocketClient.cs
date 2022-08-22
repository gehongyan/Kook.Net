using Kook.API;
using Kook.Rest;

namespace Kook.WebSocket;

public abstract partial class BaseSocketClient : BaseKookClient
{
    protected readonly KookSocketConfig BaseConfig;

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
        : base(config, client) => BaseConfig = config;

    /// <summary>
    ///     Gets a guild.
    /// </summary>
    /// <param name="id">The guild identifier.</param>
    /// <returns>
    ///     A WebSocket-based guild associated with the identifier; <c>null</c> when the guild cannot be
    ///     found.
    /// </returns>
    public abstract SocketGuild GetGuild(ulong id);

    public abstract Task StartAsync();
    public abstract Task StopAsync();

    public abstract SocketChannel GetChannel(ulong id);

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
}