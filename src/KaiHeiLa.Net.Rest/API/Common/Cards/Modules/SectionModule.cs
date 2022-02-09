using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class SectionModule : ModuleBase
{
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(SectionAccessoryModeConverter))]
    public SectionAccessoryMode Mode { get; set; }

    [JsonPropertyName("text")]
    public ElementBase Text { get; set; }

    [JsonPropertyName("accessory")]
    public ElementBase Accessory { get; set; }
}