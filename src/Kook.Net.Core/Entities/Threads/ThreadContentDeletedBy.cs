namespace Kook;

/// <summary>
///     表示帖子被删除的操作者。
/// </summary>
public enum ThreadContentDeletedBy
{
    /// <summary>
    ///     帖子未被删除。
    /// </summary>
    None = 0,

    /// <summary>
    ///     由作者自己删除。
    /// </summary>
    Author = 1,

    /// <summary>
    ///     由管理员删除。
    /// </summary>
    Moderator = 2,

    /// <summary>
    ///     由审核删除。
    /// </summary>
    Audit = 3
}
