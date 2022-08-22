using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ButtonElement : ElementBase
{
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(ButtonThemeConverter))]
    public ButtonTheme Theme { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; }

    [JsonPropertyName("click")]
    [JsonConverter(typeof(ButtonClickEventTypeConverter))]
    public ButtonClickEventType Click { get; set; }

    [JsonPropertyName("text")]
    public ElementBase Text { get; set; }
}