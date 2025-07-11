namespace Kook;

/// <summary>
///     表示一个帖子的审核状态。
/// </summary>
public enum ThreadAuditState
{
    /// <summary>
    ///     帖子正在审核中。
    /// </summary>
    Auditing = 1,

    /// <summary>
    ///     帖子审核通过。
    /// </summary>
    Approved = 2,

    /// <summary>
    ///     帖子编辑审核中。
    /// </summary>
    EditingAuditing = 3
}
