using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewaySystemEventExtraData
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("body")]
    public JsonElement Body { get; set; }
}
