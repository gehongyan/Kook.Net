using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Card : CardBase
{
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(CardThemeConverter))]
    public CardTheme? Theme { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexColorConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? Color { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(CardSizeConverter))]
    public CardSize? Size { get; set; }

    [JsonPropertyName("modules")]
    public required ModuleBase[] Modules { get; set; }
}
