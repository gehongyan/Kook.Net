using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewaySocketFrame
{
    [JsonPropertyName("s")]
    public GatewaySocketFrameType Type { get; set; }

    [JsonPropertyName("sn")]
    public int? Sequence { get; set; }

    [JsonPropertyName("d")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Payload { get; set; }
}
