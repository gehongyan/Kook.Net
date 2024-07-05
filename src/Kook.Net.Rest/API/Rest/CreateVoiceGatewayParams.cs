using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateVoiceGatewayParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("audio_ssrc")]
    public uint? Ssrc { get; set; }

    [JsonPropertyName("audio_pt")]
    public byte? PayloadType { get; set; }

    [JsonPropertyName("rtcp_mux")]
    public bool? RtcpMultiplexing { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
