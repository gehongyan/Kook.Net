namespace Kook;

/// <summary>
///     Represents a set of json error codes received by Kook.
/// </summary>
public enum KookErrorCode
{
    /// <summary>
    ///     The operation was successful.
    /// </summary>
    Success = 0,

    /// <summary>
    ///     The operation failed due to an unspecified error.
    /// </summary>
    GeneralError = 40000,

    #region Hello

    /// <summary>
    ///     The operation failed due to an missing argument.
    /// </summary>
    MissingArgument = 40100,

    /// <summary>
    ///     The operation failed due to an invalid authentication token.
    /// </summary>
    InvalidAuthenticationToken = 40101,

    /// <summary>
    ///     The operation failed because the authentication token verification failed.
    /// </summary>
    TokenVerificationFailed = 40102,

    /// <summary>
    ///     The operation failed because the authentication token has expired.
    /// </summary>
    TokenExpired = 40103,

    #endregion

    /// <summary>
    ///     The operation failed because the request was too large.
    /// </summary>
    RequestEntityTooLarge = 40014,

    #region Reconnect

    /// <summary>
    ///     The KOOK gateway requested a reconnect due to missing resume arguments.
    /// </summary>
    MissingResumeArgument = 40106,

    /// <summary>
    ///     The KOOK gateway requested a reconnect because the session has expired.
    /// </summary>
    SessionExpired = 40107,

    /// <summary>
    ///     The KOOK gateway requested a reconnect due to an invalid sequence number.
    /// </summary>
    InvalidSequenceNumber = 40108,

    #endregion

    /// <summary>
    ///     The operation failed due to missing permissions.
    /// </summary>
    MissingPermissions = 40300,

    #region Friends

    /// <summary>
    ///     The operation failed because the user has become a friend of the current user.
    /// </summary>
    HasBeenFriend = 42007,

    /// <summary>
    ///     The operation failed because the current user has requested to be friends with the user too fast.
    /// </summary>
    RequestFriendTooFast = 42008,

    #endregion
}
