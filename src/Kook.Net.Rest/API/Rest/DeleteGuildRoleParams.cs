using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGuildRoleParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("role_id")]
    public required uint Id { get; set; }
}
