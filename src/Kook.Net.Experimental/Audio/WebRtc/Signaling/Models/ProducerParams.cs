using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class ProducerParams
{
    [JsonPropertyName("producerId")]
    public required string ProducerId { get; set; }
}
