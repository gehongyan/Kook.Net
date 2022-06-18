using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class GetVoiceGatewayResponse
{
    [JsonPropertyName("gateway_url")]
    public string Url { get; set; }
}