using System.Text.Json.Serialization;

namespace Kook.API;

internal class KpmVipInfo
{
    [JsonPropertyName("exp")]
    public required int Exp { get; set; }

    [JsonPropertyName("discount")]
    public required decimal Discount { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("level")]
    public required int Level { get; set; }
}
