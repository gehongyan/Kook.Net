using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewaySocketFrame
{
    [JsonPropertyName("s")]
    [JsonConverter(typeof(GatewaySocketFrameTypeConverter))]
    public GatewaySocketFrameType Type { get; set; }

    [JsonPropertyName("sn")]
    public int? Sequence { get; set; }

    [JsonPropertyName("d")]
    public object Payload { get; set; }
}