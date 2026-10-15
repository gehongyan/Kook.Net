using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     <c>startAccompaniment</c> 通知的载荷。
/// </summary>
internal class VoiceAccompanimentData
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    [JsonPropertyName("data")]
    public VoiceAccompanimentInfo? Data { get; set; }
}
