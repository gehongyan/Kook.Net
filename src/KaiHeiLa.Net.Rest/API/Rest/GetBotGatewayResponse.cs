using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class GetBotGatewayResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}