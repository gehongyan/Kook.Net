using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceDtlsParameters
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("fingerprints")]
    public required IReadOnlyList<VoiceDtlsFingerprint> Fingerprints { get; set; }
}
