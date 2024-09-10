using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class AddOrRemoveRoleResponse
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("roles")]
    public required uint[] RoleIds { get; set; }
}
