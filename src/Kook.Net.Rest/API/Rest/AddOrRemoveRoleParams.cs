using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class AddOrRemoveRoleParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("role_id")]
    public required uint RoleId { get; set; }
}
