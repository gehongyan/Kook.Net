using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class VoiceSocketRequestFrame
{
    [JsonPropertyName("request")]
    public bool Request { get; set; }

    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("method")]
    [JsonConverter(typeof(VoiceSocketFrameTypeConverter))]
    public VoiceSocketFrameType Type { get; set; }

    [JsonPropertyName("data")]
    public object Payload { get; set; }
}
