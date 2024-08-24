namespace Kook;

/// <summary>
///     表示一个通用的嵌套频道，即可以嵌套在分组频道中的服务器频道。
/// </summary>
public interface INestedChannel : IGuildChannel
{
    #region General

    /// <summary>
    ///     获取此嵌套频道在服务器频道列表中所属的分组频道的 ID。
    /// </summary>
    /// <remarks> 如果当前频道不属于任何分组频道，则会返回 <c>null</c>。 </remarks>
    ulong? CategoryId { get; }

    /// <summary>
    ///     指示此嵌套频道的权限是否与其所属分组频道同步。
    /// </summary>
    /// <remarks>
    ///     如果权限同步，则此属性返回 <c>true</c>；如果权限不同步，则返回 <c>false</c>；如果无法确定权限是否同步，则返回 <c>null</c>。
    /// </remarks>
    bool? IsPermissionSynced { get; }

    /// <summary>
    ///     同步此嵌套频道的权限配置与其所属分组频道一致，并保持同步。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步频道权限同步操作的任务。
    /// </returns>
    Task SyncPermissionsAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取此频道的所属分组频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此频道所属的分组频道，如果当前频道不属于任何分组频道，则为 <c>null</c>。 </returns>
    Task<ICategoryChannel?> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Invites

    /// <summary>
    ///     获取此嵌套频道的所有邀请信息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此频道中找到的所有邀请信息。 </returns>
    Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null);

    /// <summary>
    ///     创建一个到此频道新邀请。
    /// </summary>
    /// <param name="maxAge"> 邀请链接的有效时长，<see cref="F:Kook.InviteMaxAge.NeverExpires"/> 表示永不过期。 </param>
    /// <param name="maxUses"> 邀请链接的可用人次，<see cref="F:Kook.InviteMaxUses.Unlimited"/> 表示无限制。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步邀请创建操作的任务。任务的结果包含所创建的邀请链接的元数据，其中包含有关邀请链接的信息。 </returns>
    Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited,
        RequestOptions? options = null);

    /// <summary>
    ///     创建一个到此频道新邀请。
    /// </summary>
    /// <param name="maxAge"> 邀请链接的有效时长，<c>null</c> 表示永不过期。 </param>
    /// <param name="maxUses">邀请链接的可用人次，<c>null</c> 表示无限制。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步邀请创建操作的任务。任务的结果包含所创建的邀请链接的元数据，其中包含有关邀请链接的信息。 </returns>
    Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions? options = null);

    #endregion
}
