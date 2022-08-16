using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewayReconnectPayload
{
    [JsonPropertyName("code")]
    public KaiHeiLaErrorCode Code { get; set; }
    
    [JsonPropertyName("err")]
    public string Message { get; set; }
}