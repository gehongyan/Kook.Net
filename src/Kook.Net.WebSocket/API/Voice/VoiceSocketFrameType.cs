namespace Kook.API.Voice;

internal enum VoiceSocketFrameType
{
    #region Request & Response

    GetRouterRtpCapabilities,
    Join,
    CreatePlainTransport,
    Produce,

    #endregion

    #region Notification

    NewPeer,
    PeerClosed,
    ResumeHeadset,
    PauseHeadset,
    ConsumerResumed,
    ConsumerPaused,
    PeerPermissionChanged,

    Disconnect

    #endregion
}
