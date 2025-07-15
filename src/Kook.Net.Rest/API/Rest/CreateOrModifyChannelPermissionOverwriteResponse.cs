using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateOrModifyChannelPermissionOverwriteResponse
{
    [JsonPropertyName("role_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? RoleId { get; set; }

    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public User? User { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }
}
