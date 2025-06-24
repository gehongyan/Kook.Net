namespace Kook;

/// <summary>
///     表示从 KOOK 接收到的错误代码。
/// </summary>
public enum KookErrorCode
{
    /// <summary>
    ///     操作成功。
    /// </summary>
    Success = 0,

    /// <summary>
    ///     操作失败，原因未通过错误代码明确。
    /// </summary>
    GeneralError = 40000,

    /// <summary>
    ///     操作失败，要求检查消息内容后重试。
    /// </summary>
    InvalidMessageContent = 40011,

    #region Hello

    /// <summary>
    ///     操作由于缺少参数而失败。
    /// </summary>
    MissingArgument = 40100,

    /// <summary>
    ///     操作由于无效的身份验证令牌而失败。
    /// </summary>
    InvalidAuthenticationToken = 40101,

    /// <summary>
    ///     操作由于身份验证令牌验证失败而失败。
    /// </summary>
    TokenVerificationFailed = 40102,

    /// <summary>
    ///     操作由于身份验证令牌已过期而失败。
    /// </summary>
    TokenExpired = 40103,

    #endregion

    /// <summary>
    ///     操作由于请求传输的实体过大而失败。
    /// </summary>
    RequestEntityTooLarge = 40014,

    #region Reconnect

    /// <summary>
    ///     KOOK 网关由于缺少恢复参数而要求重新连接。
    /// </summary>
    MissingResumeArgument = 40106,

    /// <summary>
    ///     KOOK 网关由于会话已过期而要求重新连接。
    /// </summary>
    SessionExpired = 40107,

    /// <summary>
    ///     KOOK 网关由于无效的消息序号而要求重新连接。
    /// </summary>
    InvalidSequenceNumber = 40108,

    #endregion

    /// <summary>
    ///     操作由于缺少权限而失败。
    /// </summary>
    MissingPermissions = 40300,

    #region Friends

    /// <summary>
    ///     操作由于用户已是当前用户的好友而失败。
    /// </summary>
    HasBeenFriend = 42007,

    /// <summary>
    ///     操作由于当前用户请求与用户成为好友过于频繁而失败。
    /// </summary>
    RequestFriendTooFast = 42008,

    /// <summary>
    ///     操作操作由于当前用户不为 BUFF 用户而失败。
    /// </summary>
    MissingBuffForRequestingIntimacyRelation = 42012

    #endregion
}
