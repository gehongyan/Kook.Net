using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     只携带成员 ID 的通知载荷，用于 <c>peerClosed</c>、<c>consumerPaused</c>、<c>consumerResumed</c>、
///     <c>pauseHeadset</c>、<c>resumeHeadset</c> 和 <c>stopAccompaniment</c>。
///     其中 <c>stopAccompaniment</c> 的载荷在实测中未出现过，这里假定它与 <c>startAccompaniment</c> 一样带有 <c>peerId</c>。
/// </summary>
internal class VoicePeerRefData
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }
}
