using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGuildParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    public static implicit operator DeleteGuildParams(ulong guildId) => new() { GuildId = guildId };
}
