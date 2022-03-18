using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class SelfGuildEvent
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong GuildId { get; set; }
}