using System.Text.Json.Serialization;

namespace Kook.API.Webhook;

internal class GatewayEncryptedFrame
{
    [JsonPropertyName("encrypt")]
    public required string Encrypted { get; set; }
}
