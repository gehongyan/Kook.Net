namespace Kook;

/// <summary>
///     表示一个用户之间的好友关系状态。
/// </summary>
public enum FriendState
{
    /// <summary>
    ///     表示一个尚未被接受的待处理好友请求。
    /// </summary>
    Pending,

    /// <summary>
    ///     表示一个已接受的好友请求，即该用户已被添加到当前用户的好友列表中。
    /// </summary>
    Accepted,

    /// <summary>
    ///     表示一个已屏蔽的好友状态，即该用户已被当前用户屏蔽。
    /// </summary>
    Blocked
}
