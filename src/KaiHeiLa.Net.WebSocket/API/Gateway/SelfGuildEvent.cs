using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class SelfGuildEvent
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }
}