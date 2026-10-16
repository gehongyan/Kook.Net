namespace Kook.Audio.WebRtc.Media;

/// <summary>
///     从本端 SDP offer 中取得、需要通过 <c>initSpeaking</c> 告知服务端的信息。
/// </summary>
internal sealed record WebRtcLocalDescription(
    string Mid,
    uint Ssrc,
    string Cname,
    string FingerprintAlgorithm,
    string Fingerprint);
