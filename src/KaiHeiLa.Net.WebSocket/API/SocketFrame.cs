using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class SocketFrame
{
    [JsonPropertyName("s")]
    [JsonConverter(typeof(SocketFrameTypeConverter))]
    public SocketFrameType Type { get; set; }

    [JsonPropertyName("sn")]
    public int? Sequence { get; set; }

    [JsonPropertyName("d")]
    public object Payload { get; set; }
}