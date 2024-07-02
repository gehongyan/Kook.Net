namespace Kook;

/// <summary>
///     表示客户端的登录状态。
/// </summary>
public enum LoginState : byte
{
    /// <summary>
    ///     已退出登录。
    /// </summary>
    LoggedOut,

    /// <summary>
    ///     正在登录。
    /// </summary>
    LoggingIn,

    /// <summary>
    ///     已登录。
    /// </summary>
    LoggedIn,

    /// <summary>
    ///     正在退出登录。
    /// </summary>
    LoggingOut
}
