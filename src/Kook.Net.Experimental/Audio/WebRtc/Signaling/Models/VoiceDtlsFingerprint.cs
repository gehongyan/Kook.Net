using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceDtlsFingerprint
{
    [JsonPropertyName("algorithm")]
    public required string Algorithm { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }
}
