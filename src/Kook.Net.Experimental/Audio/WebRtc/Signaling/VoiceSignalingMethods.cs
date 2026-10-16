namespace Kook.Audio.WebRtc.Signaling;

internal static class VoiceSignalingMethods
{
    #region 客户端 → 服务端 请求

    public const string QuickJoin = "quickJoin";
    public const string CreateTurnTransport = "createTurnTransport";
    public const string InitSpeaking = "initSpeaking";
    public const string PauseProducer = "pauseProducer";
    public const string ResumeProducer = "resumeProducer";
    public const string CloseProducer = "closeProducer";

    #endregion

    #region 服务端 → 客户端 请求

    public const string NewConsumer = "newConsumer";
    public const string ActiveSpeaker = "active-speaker";
    public const string CloseByOtherDevice = "closeByOtherDevice";
    public const string Reload = "reload";

    #endregion

    #region 服务端 → 客户端 通知

    public const string NewPeer = "newPeer";
    public const string PeerClosed = "peerClosed";
    public const string ConsumerPaused = "consumerPaused";
    public const string ConsumerResumed = "consumerResumed";
    public const string ConsumerClosed = "consumerClosed";
    public const string PauseHeadset = "pauseHeadset";
    public const string ResumeHeadset = "resumeHeadset";
    public const string PeerPermissionChanged = "peerPermissionChanged";
    public const string Atmosphere = "atmosphere";
    public const string StartAccompaniment = "startAccompaniment";
    public const string StopAccompaniment = "stopAccompaniment";
    public const string Disconnect = "disconnect";

    #endregion
}
