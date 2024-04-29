using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGameParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("process_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProcessName { get; set; }

    [JsonPropertyName("icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Icon { get; set; }
}
