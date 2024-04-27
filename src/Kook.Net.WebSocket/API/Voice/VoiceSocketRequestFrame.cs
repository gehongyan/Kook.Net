using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class VoiceSocketRequestFrame
{
    [JsonPropertyName("request")]
    public required bool Request { get; set; }

    [JsonPropertyName("id")]
    public required uint Id { get; set; }

    [JsonPropertyName("method")]
    [JsonConverter(typeof(VoiceSocketFrameTypeConverter))]
    public required VoiceSocketFrameType Type { get; set; }

    [JsonPropertyName("data")]
    public required object Payload { get; set; }
}
