namespace KaiHeiLa;

/// <summary>
///     分组内的频道
/// </summary>
public interface INestedChannel : IGuildChannel
{
    #region General

    /// <summary>
    ///     所属分组的 ID
    /// </summary>
    ulong? CategoryId { get; }
    
    /// <summary>
    ///     权限设置是否与分组同步
    /// </summary>
    bool IsPermissionSynced { get; }
    
    /// <summary>
    ///     Gets the parent (category) channel of this channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the category channel
    ///     representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    #endregion

    #region Invites

    /// <summary>
    ///     Gets a collection of all invites from this guild channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     invite, each representing information for an invite found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null);

    #endregion
    
}