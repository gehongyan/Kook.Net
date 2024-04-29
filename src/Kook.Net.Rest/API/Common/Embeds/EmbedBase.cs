using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

[JsonConverter(typeof(EmbedConverter))]
internal class EmbedBase : IEmbed
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(EmbedTypeConverter))]
    public EmbedType Type { get; set; }
}
