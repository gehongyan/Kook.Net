namespace Kook;

/// <summary>
///     Represents a generic guild channel.
/// </summary>
public interface IGuildChannel : IChannel, IDeletable
{
    #region General

    /// <summary>
    ///     Gets the guild associated with this channel.
    /// </summary>
    /// <returns>
    ///     A guild object that this channel belongs to.
    /// </returns>
    IGuild Guild { get; }

    /// <summary>
    ///     Gets the guild ID associated with this channel.
    /// </summary>
    /// <returns>
    ///     An <see langword="ulong"/> representing the guild identifier for the guild that this channel
    ///     belongs to.
    /// </returns>
    ulong GuildId { get; }

    /// <summary>
    ///     Gets the position of this channel.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> representing the position of this channel in the guild's channel list relative to
    ///     others of the same type.
    /// </returns>
    int? Position { get; }

    /// <summary>
    ///     Gets the type of this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="ChannelType"/> representing the type of this channel.
    /// </returns>
    ChannelType Type { get; }

    /// <summary>
    ///     Gets the identifier of the user who created this channel.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong"/> representing the identifier of the user who created this channel.
    /// </returns>
    ulong CreatorId { get; }

    /// <summary>
    ///     Gets a collection of permission overwrites for roles for this channel.
    /// </summary>
    /// <returns>
    ///     A collection of overwrites for roles associated with this channel.
    /// </returns>
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }

    /// <summary>
    ///     Gets a collection of permission overwrites for users for this channel.
    /// </summary>
    /// <returns>
    ///     A collection of overwrites for users associated with this channel.
    /// </returns>
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }

    /// <summary>
    ///     Modifies this guild channel.
    /// </summary>
    /// <remarks>
    ///     This method modifies the current guild channel with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="ModifyGuildChannelProperties"/>.
    /// </remarks>
    /// <param name="func">The delegate containing the properties to modify the channel with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions options = null);

    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    Task<IUser> GetCreatorAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

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
    Task ModifyPermissionOverwriteAsync(IRole role, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options = null);

    /// <summary>
    ///     Updates the permission overwrite for the given user.
    /// </summary>
    /// <param name="user">The user to add the overwrite to.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
    /// </returns>
    Task ModifyPermissionOverwriteAsync(IGuildUser user, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options = null);

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
