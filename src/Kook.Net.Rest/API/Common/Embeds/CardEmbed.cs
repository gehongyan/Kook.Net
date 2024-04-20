using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class CardEmbed : EmbedBase
{
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(CardThemeConverter))]
    public CardTheme Theme { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexColorConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? Color { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(CardSizeConverter))]
    public CardSize Size { get; set; }

    [JsonPropertyName("modules")]
    public ModuleBase[] Modules { get; set; }
}
