using System.Text.Json.Serialization;

namespace Kook.API;

internal class ThreadTag
{
    [JsonPropertyName("id")] public uint Id { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("icon")] public required string Icon { get; set; }
}
