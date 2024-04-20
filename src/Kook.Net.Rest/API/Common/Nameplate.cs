using System.Text.Json.Serialization;

namespace Kook.API;

internal class Nameplate
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required int Type { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("tips")]
    public required string Tips { get; set; }

    [JsonPropertyName("h")]
    public int? Height { get; set; }

    [JsonPropertyName("w")]
    public int? Width { get; set; }
}
