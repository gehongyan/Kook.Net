using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceIceParameters
{
    [JsonPropertyName("usernameFragment")]
    public required string UsernameFragment { get; set; }

    [JsonPropertyName("password")]
    public required string Password { get; set; }

    [JsonPropertyName("iceLite")]
    public bool? IceLite { get; set; }
}
