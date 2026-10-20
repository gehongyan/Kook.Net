using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class QuickJoinResponse
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    [JsonPropertyName("pTransport")]
    public required VoiceTransportParameters ProducerTransport { get; set; }

    [JsonPropertyName("cTransport")]
    public required VoiceTransportParameters ConsumerTransport { get; set; }

    [JsonPropertyName("rtpConfig")]
    public VoiceRtpConfig? RtpConfig { get; set; }
}
