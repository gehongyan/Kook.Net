namespace Kook.API.Voice;

internal readonly struct VoiceSocketFrameType(string value)
{
    public string Value { get; } = value;

    #region Request & Response

    public const string GetRouterRtpCapabilities = "getRouterRtpCapabilities";
    public const string Join = "join";
    public const string CreatePlainTransport = "createPlainTransport";
    public const string Produce = "produce";

    #endregion

    #region Notification

    public const string NewPeer = "newPeer";
    public const string PeerClosed = "peerClosed";
    public const string ResumeHeadset = "resumeHeadset";
    public const string PauseHeadset = "pauseHeadset";
    public const string ConsumerResumed = "consumerResumed";
    public const string ConsumerPaused = "consumerPaused";
    public const string PeerPermissionChanged = "peerPermissionChanged";
    public const string Atmosphere = "atmosphere";
    public const string StartAccompaniment = "startAccompaniment";
    public const string StopAccompaniment = "stopAccompaniment";

    public const string Disconnect = "disconnect";

    #endregion

    public static implicit operator string (VoiceSocketFrameType frameType) => frameType.Value;
    public static implicit operator VoiceSocketFrameType(string value) => new(value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
