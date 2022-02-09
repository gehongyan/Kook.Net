using KaiHeiLa.API;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public abstract partial class BaseSocketClient : BaseKaiHeiLaClient
{
    protected readonly KaiHeiLaSocketConfig BaseConfig;
    
    /// <summary>
    ///     Gets the estimated round-trip latency, in milliseconds, to the gateway server.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> that represents the round-trip latency to the WebSocket server. Please
    ///     note that this value does not represent a "true" latency for operations such as sending a message.
    /// </returns>
    public abstract int Latency { get; protected set; }

    /// <summary>
    ///     Provides access to a REST-only client with a shared state from this client.
    /// </summary>
    public abstract KaiHeiLaSocketRestClient Rest { get; }
    
    internal new KaiHeiLaSocketApiClient ApiClient => base.ApiClient as KaiHeiLaSocketApiClient;
    
    /// <summary>
    ///     Gets the current logged-in user.
    /// </summary>
    public new virtual SocketSelfUser CurrentUser { get => base.CurrentUser as SocketSelfUser; protected set => base.CurrentUser = value; }
    /// <summary>
    ///     Gets a collection of guilds that the user is currently in.
    /// </summary>
    /// <returns>
    ///     A read-only collection of guilds that the current user is in.
    /// </returns>
    public abstract IReadOnlyCollection<SocketGuild> Guilds { get; }
    
    internal BaseSocketClient(KaiHeiLaSocketConfig config, KaiHeiLaRestApiClient client)
        : base(config, client) => BaseConfig = config;
    
    /// <summary>
    ///     Gets a guild.
    /// </summary>
    /// <param name="id">The guild identifier.</param>
    /// <returns>
    ///     A WebSocket-based guild associated with the snowflake identifier; <c>null</c> when the guild cannot be
    ///     found.
    /// </returns>
    public abstract SocketGuild GetGuild(ulong id);
    
    public abstract Task StartAsync();
    public abstract Task StopAsync();

    public abstract SocketChannel GetChannel(ulong id);
    
}