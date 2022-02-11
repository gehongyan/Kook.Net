namespace KaiHeiLa;

public interface IGuildChannel : IChannel
{
    IGuild Guild { get; }
    
    ulong GuildId { get; }

    int Position { get; }

    ChannelType Type { get; }
    
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }
    
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }
    
    /// <summary>
    ///     Gets the permission overwrite for a specific role.
    /// </summary>
    /// <param name="role">The role to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted role; <c>null</c> if none is set.
    /// </returns>
    OverwritePermissions GetPermissionOverwrite(IRole role);
    /// <summary>
    ///     Gets the permission overwrite for a specific user.
    /// </summary>
    /// <param name="user">The user to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted user; <c>null</c> if none is set.
    /// </returns>
    OverwritePermissions GetPermissionOverwrite(IUser user);
    /// <summary>
    ///     Gets a user in this channel.
    /// </summary>
    /// <param name="id">The snowflake identifier of the user.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous get operation. The task result contains a guild user object that
    ///     represents the user; <c>null</c> if none is found.
    /// </returns>
    new Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

}