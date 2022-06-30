using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class CreateOrModifyChannelPermissionOverwriteResponse
{
    [JsonPropertyName("role_id")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? RoleId { get; set; }
    [JsonPropertyName("user_id")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonNumberHandling(JsonNumberHandling.Strict)]
    public ulong? UserId { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public PermissionOverwriteTargetType TargetType
    {
        get
        {
            if (RoleId is not null) return PermissionOverwriteTargetType.Role;
            if (UserId is not null) return PermissionOverwriteTargetType.User;
            return PermissionOverwriteTargetType.Unspecified;
        }
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public ulong TargetId => TargetType switch
    {
        PermissionOverwriteTargetType.Role => RoleId ?? 0,
        PermissionOverwriteTargetType.User => UserId ?? 0,
        _ => 0
    };

    public ulong Allow { get; set; }
    [JsonPropertyName("deny")] public ulong Deny { get; set; }
}