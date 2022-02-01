using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class GetGatewayResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}