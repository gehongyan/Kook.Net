using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class Card : CardBase
{
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(CardThemeConverter))]
    public CardTheme Theme { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(NullableColorConverter))]
    public Color? Color { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(CardSizeConverter))]
    public CardSize Size { get; set; }

    [JsonPropertyName("modules")]
    public ModuleBase[] Modules { get; set; }
}