using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateVoiceGatewayResponse
{
    [JsonPropertyName("ip")]
    public required string Ip { get; set; }

    [JsonPropertyName("port")]
    public required int Port { get; set; }

    [JsonPropertyName("rtcp_port")]
    public required int RtcpPort { get; set; }

    [JsonPropertyName("rtcp_mux")]
    public required bool RtcpMultiplexing { get; set; }

    [JsonPropertyName("bitrate")]
    public required int Bitrate { get; set; }

    [JsonPropertyName("audio_ssrc")]
    public required uint Ssrc { get; set; }

    [JsonPropertyName("audio_pt")]
    public required byte PayloadType { get; set; }
}
