using System.Text.Json.Serialization;

namespace Kook.API;

internal class UserPermissionOverwrite
{
    [JsonPropertyName("user")] public User User { get; set; }
    [JsonPropertyName("allow")] public ulong Allow { get; set; }
    [JsonPropertyName("deny")] public ulong Deny { get; set; }
}