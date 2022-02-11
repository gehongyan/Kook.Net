namespace KaiHeiLa
{
    /// <summary>
    ///     Represents a set of json error codes received by KaiHeiLa.
    /// </summary>
    public enum KaiHeiLaErrorCode
    {
        Success = 0,
        GeneralError = 40000,

        #region Hello

        MissingArgument = 40100,
        InvalidAuthenticationToken = 40101,
        TokenVerificationFailed = 40102,
        TokenExpired = 40103,

        #endregion

        RequestEntityTooLarge = 40014,
        
        #region Reconnect

        MissingResumeArgument = 40106,
        SessionExpired = 40107,
        InvalidSequenceNumber = 40108,

        #endregion
        
        MissingPermissions = 40300
    }
}
