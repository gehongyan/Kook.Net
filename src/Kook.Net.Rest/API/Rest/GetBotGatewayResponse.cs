using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetBotGatewayResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
