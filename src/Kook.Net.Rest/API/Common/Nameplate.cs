using System.Text.Json.Serialization;

namespace Kook.API;

internal class Nameplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("tips")]
    public string Tips { get; set; }
}
