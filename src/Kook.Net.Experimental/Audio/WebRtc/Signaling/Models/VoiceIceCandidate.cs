using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceIceCandidate
{
    [JsonPropertyName("foundation")]
    public required string Foundation { get; set; }

    [JsonPropertyName("ip")]
    public required string Ip { get; set; }

    [JsonPropertyName("port")]
    public required int Port { get; set; }

    [JsonPropertyName("priority")]
    public required uint Priority { get; set; }

    [JsonPropertyName("protocol")]
    public required string Protocol { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("tcpType")]
    public string? TcpType { get; set; }
}
