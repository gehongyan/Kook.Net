using System.Text.Json.Serialization;

namespace Kook.API;

internal class Poke
{
    [JsonPropertyName("id")]
    public required uint Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("desc")]
    public required string Description { get; set; }

    [JsonPropertyName("cd")]
    public required int Cooldown { get; set; }

    [JsonPropertyName("categories")]
    public required string[] Categories { get; set; }

    [JsonPropertyName("label")]
    public required uint LabelId { get; set; }

    [JsonPropertyName("label_name")]
    public required string LabelName { get; set; }

    [JsonPropertyName("quality")]
    public required uint QualityId { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("icon_expired")]
    public required string IconExpired { get; set; }

    [JsonPropertyName("quality_resource")]
    public required PokeQualityResource Quality { get; set; }

    [JsonPropertyName("resources")]
    public required PokeResourceBase Resource { get; set; }

    [JsonPropertyName("msg_scenarios")]
    public required Dictionary<string, string> MessageScenarios { get; set; }
}
