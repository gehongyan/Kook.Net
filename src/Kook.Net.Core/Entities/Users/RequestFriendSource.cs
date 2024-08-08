namespace Kook;

/// <summary>
///     表示一个好友请求的来源。
/// </summary>
public enum RequestFriendSource
{
    /// <summary>
    ///     请求发起自用户名及其标识号。
    /// </summary>
    FullQualification = 0,

    /// <summary>
    ///     请求发起自共同加入的服务器。
    /// </summary>
    Guild = 2
}
