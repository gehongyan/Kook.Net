using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGuildRoleParams
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role_id")]
    public uint Id { get; set; }
}
