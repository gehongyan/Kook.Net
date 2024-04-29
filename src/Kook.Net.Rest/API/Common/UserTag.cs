using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class UserTag
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("bg_color")]
    [JsonConverter(typeof(HexAlphaColorConverter))]
    public AlphaColor BackgroundColor { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }
}
