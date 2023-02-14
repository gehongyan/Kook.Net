using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class AddOrRemoveRoleParams
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("role_id")]
    public uint RoleId { get; set; }
}
