using System.Text.Json.Serialization;

namespace Kook.API;

internal class Poke
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("desc")]
    public string Description { get; set; }
    [JsonPropertyName("cd")]
    public int Cooldown { get; set; }
    [JsonPropertyName("categories")]
    public string[] Categories { get; set; }
    [JsonPropertyName("label")]
    public uint LabelId { get; set; }
    [JsonPropertyName("label_name")]
    public string LabelName { get; set; }
    [JsonPropertyName("quality")]
    public uint QualityId { get; set; }
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    [JsonPropertyName("icon_expired")]
    public string IconExpired { get; set; }
    [JsonPropertyName("quality_resource")]
    public PokeQualityResource Quality { get; set; }
    [JsonPropertyName("resources")]
    public PokeResourceBase Resource { get; set; }
    [JsonPropertyName("msg_scenarios")]
    public Dictionary<string, string> MessageScenarios { get; set; }
}
