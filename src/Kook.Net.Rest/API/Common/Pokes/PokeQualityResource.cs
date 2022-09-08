using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class PokeQualityResource
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color Color { get; set; }
    [JsonPropertyName("small")]
    public string Small { get; set; }
    [JsonPropertyName("big")]
    public string Big { get; set; }
}