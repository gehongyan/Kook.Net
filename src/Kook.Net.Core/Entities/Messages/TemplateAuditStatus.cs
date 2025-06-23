namespace Kook;

/// <summary>
///     表示消息模板的审核状态。
/// </summary>
public enum TemplateAuditStatus
{
    /// <summary>
    ///     未审核。
    /// </summary>
    Pending = 0,

    /// <summary>
    ///     审核中。
    /// </summary>
    UnderReview = 1,

    /// <summary>
    ///     审核通过。
    /// </summary>
    Approved = 2,

    /// <summary>
    ///     审核拒绝。
    /// </summary>
    Rejected = 3
}