using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class UserTag
{
    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}