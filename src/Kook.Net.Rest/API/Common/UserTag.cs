using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class UserTag
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
