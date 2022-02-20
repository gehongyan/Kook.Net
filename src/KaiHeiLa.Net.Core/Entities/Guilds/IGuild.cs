namespace KaiHeiLa;

public interface IGuild : IULongEntity
{
    string Name { get; }

    string Topic { get; }

    ulong OwnerId { get; }

    string Icon { get; }

    NotifyType NotifyType { get; }

    string Region { get; }

    bool IsOpenEnabled { get; }

    uint OpenId { get; }

    ulong DefaultChannelId { get; }

    ulong WelcomeChannelId { get; }
    
    /// <summary>
    ///     Determines if this guild is currently connected and ready to be used.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         This property only applies to a WebSocket-based client.
    ///     </note>
    ///     This boolean is used to determine if the guild is currently connected to the WebSocket and is ready to be used/accessed.
    /// </remarks>
    /// <returns>
    ///     <c>true</c> if this guild is currently connected and ready to be used; otherwise <see langword="false"/>.
    /// </returns>
    bool Available { get; }
    
    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    /// <returns>
    ///     A role object that represents an <c>@everyone</c> role in this guild.
    /// </returns>
    IRole EveryoneRole { get; }

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    IRole GetRole(uint id);
    
    /// <summary>
    ///     Gets a user from this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves a user found within this guild.
    ///     <note>
    ///         This may return <see langword="null" /> in the WebSocket implementation due to incomplete user collection in
    ///         large guilds.
    ///     </note>
    /// </remarks>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild user
    ///     associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a channel in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the channel.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the generic channel
    ///     associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

}