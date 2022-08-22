using System.Text.Json.Serialization;

namespace Kook.API;

internal class Tag
{
    [JsonPropertyName("color")]
    public string Color { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
}