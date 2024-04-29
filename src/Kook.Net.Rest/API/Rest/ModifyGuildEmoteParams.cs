using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyGuildEmoteParams
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
