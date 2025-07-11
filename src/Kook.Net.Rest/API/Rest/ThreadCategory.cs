using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ThreadCategory
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }

    [JsonPropertyName("roles")]
    public ThreadCategoryPermissionOverwrite[]? Roles { get; set; }
}
