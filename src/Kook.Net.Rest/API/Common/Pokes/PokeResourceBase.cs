using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

[JsonConverter(typeof(PokeResourceConverter))]
internal class PokeResourceBase : IPokeResource
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(PokeResourceTypeConverter))]
    public PokeResourceType Type { get; set; }
}