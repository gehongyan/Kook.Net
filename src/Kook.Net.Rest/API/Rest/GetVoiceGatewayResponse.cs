using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetVoiceGatewayResponse
{
    [JsonPropertyName("gateway_url")]
    public string Url { get; set; }
}
