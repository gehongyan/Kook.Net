using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

[JsonConverter(typeof(PokeResourceConverter))]
internal class PokeResourceBase : IPokeResource
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(PokeResourceTypeConverter))]
    public required PokeResourceType Type { get; set; }
}
