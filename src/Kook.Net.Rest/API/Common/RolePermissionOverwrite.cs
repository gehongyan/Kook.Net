using System.Text.Json.Serialization;

namespace Kook.API;

internal class RolePermissionOverwrite
{
    [JsonPropertyName("role_id")]
    public uint RoleId { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }
}
