using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Card : CardBase
{
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(CardThemeConverter))]
    public CardTheme Theme { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color? Color { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(CardSizeConverter))]
    public CardSize Size { get; set; }

    [JsonPropertyName("modules")]
    public ModuleBase[] Modules { get; set; }
}