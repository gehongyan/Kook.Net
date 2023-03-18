using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class PokeQualityResource
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("small")]
    public string Small { get; set; }

    [JsonPropertyName("big")]
    public string Big { get; set; }
}
