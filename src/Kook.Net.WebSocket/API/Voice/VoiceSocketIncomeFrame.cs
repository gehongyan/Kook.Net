using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Voice;

internal class VoiceSocketIncomeFrame
{
    [JsonPropertyName("response")]
    public bool Response { get; set; }

    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("ok")]
    public bool Okay { get; set; }

    [JsonPropertyName("data")]
    public required object Payload { get; set; }

    [JsonPropertyName("notification")]
    public bool Notification { get; set; }

    [JsonPropertyName("method")]
    [JsonConverter(typeof(VoiceSocketFrameTypeConverter))]
    public VoiceSocketFrameType Method { get; set; }
}
