using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceConsumerAppData
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    [JsonPropertyName("isBot")]
    public bool? IsBot { get; set; }

    [JsonPropertyName("isVip")]
    public bool? IsVip { get; set; }

    [JsonPropertyName("transportId")]
    public string? TransportId { get; set; }

    [JsonPropertyName("peerInfo")]
    public VoicePeerInfo? PeerInfo { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? ExtraData { get; set; }
}
