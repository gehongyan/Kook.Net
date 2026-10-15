using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class InitSpeakingResponse
{
    [JsonPropertyName("peers")]
    public IReadOnlyList<VoicePeerInfo>? Peers { get; set; }

    [JsonPropertyName("room")]
    public VoiceRoomInfo? Room { get; set; }

    [JsonPropertyName("peer")]
    public VoicePeerInfo? Peer { get; set; }

    [JsonPropertyName("producer")]
    public VoiceProducerRef? Producer { get; set; }
}
