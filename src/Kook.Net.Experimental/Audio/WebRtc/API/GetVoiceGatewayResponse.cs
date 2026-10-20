using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetVoiceGatewayResponse
{
    [JsonPropertyName("gateway_url")]
    public required string GatewayUrl { get; set; }

    [JsonPropertyName("pc_voice_sdk")]
    public int? PcVoiceSdk { get; set; }

    [JsonPropertyName("ios_voice_sdk")]
    public int? IosVoiceSdk { get; set; }
}
