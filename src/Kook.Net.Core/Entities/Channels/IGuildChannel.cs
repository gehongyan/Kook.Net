namespace Kook;

/// <summary>
///     表示一个通用的服务器频道。
/// </summary>
public interface IGuildChannel : IChannel, IDeletable
{
    #region General

    /// <summary>
    ///     获取此频道所属的服务器。
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     获取与此频道所属的服务器的 ID。
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     获取此频道在服务器频道列表中的位置。
    /// </summary>
    /// <remarks>
    ///     更小的数值表示更靠近列表顶部的位置。
    /// </remarks>
    int? Position { get; }

    /// <summary>
    ///     获取此频道的类型。
    /// </summary>
    ChannelType Type { get; }

    /// <summary>
    ///     获取创建此频道的用户的 ID。
    /// </summary>
    ulong? CreatorId { get; }

    /// <summary>
    ///     获取此频道的角色的权限覆盖集合。
    /// </summary>
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }

    /// <summary>
    ///     获取此频道的用户的权限覆盖集合。
    /// </summary>
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }

    /// <summary>
    ///     修改此服务器频道。
    /// </summary>
    /// <param name="func"> 一个包含修改服务器频道属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示服务器频道属性修改操作的异步任务。 </returns>
    /// <seealso cref="T:Kook.ModifyGuildChannelProperties"/>
    Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     获取此频道的创建者。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果为此频道的创建者；如果没有找到则为 <c>null</c>。 </returns>
    Task<IUser?> GetCreatorAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Permissions

    /// <summary>
    ///     获取给定角色的权限覆盖配置。
    /// </summary>
    /// <param name="role"> 要获取权限覆盖配置的角色。 </param>
    /// <returns> 一个表示目标角色的权限覆盖配置；如果没有设置则为 <c>null</c>。 </returns>
    OverwritePermissions? GetPermissionOverwrite(IRole role);

    /// <summary>
    ///     获取给定用户的权限覆盖配置。
    /// </summary>
    /// <param name="user"> 要获取权限覆盖配置的用户。 </param>
    /// <returns> 一个表示目标用户的权限覆盖配置；如果没有设置则为 <c>null</c>。 </returns>
    OverwritePermissions? GetPermissionOverwrite(IUser user);

    /// <summary>
    ///     对于给定的角色，如果存在权限覆盖配置，则移除它。
    /// </summary>
    /// <param name="role"> 要对其移除权限覆盖配置的角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除频道内角色权限覆盖配置操作的任务。 </returns>
    Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null);

    /// <summary>
    ///     对于给定的用户，如果存在权限覆盖配置，则移除它。
    /// </summary>
    /// <param name="user"> 要对其移除权限覆盖配置的用户。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除频道内用户权限覆盖配置操作的任务。 </returns>
    Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null);

    /// <summary>
    ///     添加给定角色的权限覆盖配置。
    /// </summary>
    /// <param name="role"> 要添加权限覆盖配置的角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步添加频道内角色权限覆盖配置操作的任务。 </returns>
    Task AddPermissionOverwriteAsync(IRole role, RequestOptions? options = null);

    /// <summary>
    ///     添加给定用户的权限覆盖配置。
    /// </summary>
    /// <param name="user"> 要添加权限覆盖配置的用户。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步添加频道内用户权限覆盖配置操作的任务。 </returns>
    Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null);

    /// <summary>
    ///     更新给定角色的权限覆盖配置。
    /// </summary>
    /// <param name="role"> 要更新权限覆盖配置的角色。 </param>
    /// <param name="func"> 一个包含修改权限覆盖配置的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步更新频道内角色权限覆盖配置操作的任务。 </returns>
    Task ModifyPermissionOverwriteAsync(IRole role, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null);

    /// <summary>
    ///     更新给定用户的权限覆盖配置。
    /// </summary>
    /// <param name="user"> 要更新权限覆盖配置的用户。 </param>
    /// <param name="func"> 一个包含修改权限覆盖配置的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步更新频道内用户权限覆盖配置操作的任务。 </returns>
    Task ModifyPermissionOverwriteAsync(IGuildUser user, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null);

    #endregion

    #region Users

    /// <summary>
    ///     获取能够查看频道或当前在此频道中的所有用户。
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IChannel.GetUsersAsync"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的服务器用户集合的异步可枚举对象。 </returns>
    new IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此频道中的用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果为此频道中的服务器用户；如果没有找到则为 <c>null</c>。 </returns>
    new Task<IGuildUser?> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion
}
