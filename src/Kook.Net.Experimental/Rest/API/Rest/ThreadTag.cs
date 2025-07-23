using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ThreadTag
{
    [JsonPropertyName("tag_id")]
    public uint TagId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }
}
