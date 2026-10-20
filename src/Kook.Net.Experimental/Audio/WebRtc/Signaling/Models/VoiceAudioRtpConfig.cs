using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceAudioRtpConfig
{
    [JsonPropertyName("stereo")]
    public int? Stereo { get; set; }

    [JsonPropertyName("ptime")]
    public int? PacketTime { get; set; }

    [JsonPropertyName("maxaveragebitrate")]
    public int? MaxAverageBitrate { get; set; }

    [JsonPropertyName("usedtx")]
    public int? UseDtx { get; set; }
}
