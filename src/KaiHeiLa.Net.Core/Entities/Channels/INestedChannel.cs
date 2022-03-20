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
    
    // TODO: GetCategoryAsync

    #endregion

    #region Invites

    // TODO: Implement Invites

    #endregion
    
}