using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ThreadCategoryPermissionOverwrite
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(TypePermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTargetType Type { get; set; }

    [JsonPropertyName("role_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? RoleId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public ulong TargetId => Type switch
    {
        PermissionOverwriteTargetType.Role => RoleId ?? 0,
        PermissionOverwriteTargetType.User => ulong.TryParse(UserId, out ulong userId) ? userId : 0,
        _ => 0
    };

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }
}