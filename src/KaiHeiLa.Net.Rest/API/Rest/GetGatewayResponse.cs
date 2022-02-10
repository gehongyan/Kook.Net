using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class GetGatewayResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}