using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceRtpConfig
{
    [JsonPropertyName("audio")]
    public VoiceAudioRtpConfig? Audio { get; set; }

    [JsonPropertyName("video")]
    public JsonElement? Video { get; set; }
}
