using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class LeaveGuildParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    public static implicit operator LeaveGuildParams(ulong guildId) => new() { GuildId = guildId };
}
