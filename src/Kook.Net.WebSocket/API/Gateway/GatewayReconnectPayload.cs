using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GatewayReconnectPayload
{
    [JsonPropertyName("code")]
    public KookErrorCode Code { get; set; }
    
    [JsonPropertyName("err")]
    public string Message { get; set; }
}