using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewayHelloPayload
{
    [JsonPropertyName("code")]
    public KaiHeiLaErrorCode Code { get; set; }
    
    [JsonPropertyName("session_id")]
    // [JsonConverter(typeof(GuidConverter))]
    public Guid SessionId { get; set; }
}