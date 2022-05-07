using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class LeaveGuildParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    public static implicit operator LeaveGuildParams(ulong guildId) => new() {GuildId = guildId};
}