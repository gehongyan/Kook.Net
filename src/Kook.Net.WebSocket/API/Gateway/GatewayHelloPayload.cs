using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewayHelloPayload
{
    [JsonPropertyName("code")]
    public KookErrorCode Code { get; set; }

    [JsonPropertyName("session_id")]
    public Guid SessionId { get; set; }
}
