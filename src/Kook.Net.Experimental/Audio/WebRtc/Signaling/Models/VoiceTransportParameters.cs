using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceTransportParameters
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("iceParameters")]
    public required VoiceIceParameters IceParameters { get; set; }

    [JsonPropertyName("iceCandidates")]
    public required IReadOnlyList<VoiceIceCandidate> IceCandidates { get; set; }

    [JsonPropertyName("dtlsParameters")]
    public required VoiceDtlsParameters DtlsParameters { get; set; }

    [JsonPropertyName("sctpParameters")]
    public JsonElement? SctpParameters { get; set; }
}
