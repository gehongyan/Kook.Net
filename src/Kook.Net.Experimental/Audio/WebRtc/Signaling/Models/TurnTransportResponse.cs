using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class TurnTransportResponse
{
    [JsonPropertyName("urls")]
    public required IReadOnlyList<string> Urls { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
