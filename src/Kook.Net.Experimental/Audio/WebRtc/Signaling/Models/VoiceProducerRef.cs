using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceProducerRef
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}
