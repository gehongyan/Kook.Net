namespace Kook;

/// <summary>
///     表示一个用户之间的亲密关系状态。
/// </summary>
public enum IntimacyState
{
    /// <summary>
    ///     表示一个尚未被接受的待处理亲密关系请求。
    /// </summary>
    Pending = 1,

    /// <summary>
    ///     表示一个已接受的亲密关系请求，即该用户已与当前用户建立了亲密关系。
    /// </summary>
    Accepted = 2,
}