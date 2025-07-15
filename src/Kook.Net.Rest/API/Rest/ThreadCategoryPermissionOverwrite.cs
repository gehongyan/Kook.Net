using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ThreadCategoryPermissionOverwrite
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(TypePermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTarget Type { get; set; }

    [JsonPropertyName("role_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? RoleId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UserId { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }
}
