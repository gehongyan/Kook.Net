namespace KaiHeiLa;

public interface IGuildChannel : IChannel
{
    #region General
    
    IGuild Guild { get; }
    
    ulong GuildId { get; }

    int? Position { get; }

    ChannelType Type { get; }
    
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }
    
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }

    #endregion

    #region Permissions

    /// <summary>
    ///     Gets the permission overwrite for a specific role.
    /// </summary>
    /// <param name="role">The role to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted role; <c>null</c> if none is set.
    /// </returns>
    OverwritePermissions? GetPermissionOverwrite(IRole role);

    /// <summary>
    ///     Gets the permission overwrite for a specific user.
    /// </summary>
    /// <param name="user">The user to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted user; <c>null</c> if none is set.
    /// </returns>
    OverwritePermissions? GetPermissionOverwrite(IUser user);
    
    /// <summary>
    ///     Removes the permission overwrite for the given role, if one exists.
    /// </summary>
    /// <param name="role">The role to remove the overwrite from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null);

    /// <summary>
    ///     Removes the permission overwrite for the given user, if one exists.
    /// </summary>
    /// <param name="user">The user to remove the overwrite from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions options = null);

    /// <summary>
    ///     Adds the permission overwrite for the given role.
    /// </summary>
    /// <param name="role">The role to add the overwrite to.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the
    ///     channel.
    /// </returns>
    Task AddPermissionOverwriteAsync(IRole role, RequestOptions options = null);

    /// <summary>
    ///     Adds the permission overwrite for the given user.
    /// </summary>
    /// <param name="user">The user to add the overwrite to.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
    /// </returns>
    Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions options = null);

    /// <summary>
    ///     Updates the permission overwrite for the given role.
    /// </summary>
    /// <param name="role">The role to add the overwrite to.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the
    ///     channel.
    /// </returns>
    Task ModifyPermissionOverwriteAsync(IRole role, Action<OverwritePermissions> func, RequestOptions options = null);

    /// <summary>
    ///     Updates the permission overwrite for the given user.
    /// </summary>
    /// <param name="user">The user to add the overwrite to.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
    /// </returns>
    Task ModifyPermissionOverwriteAsync(IGuildUser user, Action<OverwritePermissions> func, RequestOptions options = null);
    
    #endregion

    #region Users

    /// <summary>
    ///     Gets a collection of users that are able to view the channel or are currently in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IChannel.GetUsersAsync"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of users.
    /// </returns>
    new IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    
    /// <summary>
    ///     Gets a user in this channel.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous get operation. The task result contains a guild user object that
    ///     represents the user; <c>null</c> if none is found.
    /// </returns>
    new Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    #endregion
}