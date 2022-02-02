using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class GatewaySystemEventExtraData
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("body")]
    public object Body { get; set; }
}