using System.Text.Json.Serialization;

namespace Kook.API;

internal class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public GameType Type { get; set; }

    [JsonPropertyName("options")]
    public string? Options { get; set; }

    [JsonPropertyName("kmhook_admin")]
    public bool KmHookAdmin { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("process_name")]
    public required string[] ProcessNames { get; set; }

    [JsonPropertyName("product_name")]
    public required string[] ProductNames { get; set; }
}
