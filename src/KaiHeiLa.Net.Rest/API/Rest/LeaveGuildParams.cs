using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class LeaveGuildParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong GuildId { get; set; }

    public static implicit operator LeaveGuildParams(ulong guildId) => new() {GuildId = guildId};
}