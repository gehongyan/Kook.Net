using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

[JsonConverter(typeof(EmbedConverter))]
internal class EmbedBase : IEmbed
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(EmbedTypeConverter))]
    public EmbedType Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}