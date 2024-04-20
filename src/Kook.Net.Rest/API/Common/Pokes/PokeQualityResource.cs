using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class PokeQualityResource
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexColorConverter))]
    public required Color Color { get; set; }

    [JsonPropertyName("small")]
    public required string Small { get; set; }

    [JsonPropertyName("big")]
    public required string Big { get; set; }
}
