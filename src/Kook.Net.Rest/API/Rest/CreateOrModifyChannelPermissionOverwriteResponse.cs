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

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public PermissionOverwriteTargetType TargetType => this switch
    {
        { User: not null } => PermissionOverwriteTargetType.Role,
        { RoleId: not null } => PermissionOverwriteTargetType.Role,
        _ => PermissionOverwriteTargetType.Unspecified,
    };

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public ulong TargetId => TargetType switch
    {
        PermissionOverwriteTargetType.Role => RoleId ?? 0,
        PermissionOverwriteTargetType.User => User?.Id ?? 0,
        _ => 0
    };

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }
}
