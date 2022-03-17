using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GuildMemberOfflineEvent
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("event_time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset OfflineAt { get; set; }

    [JsonPropertyName("guilds")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong[] CommonGuilds { get; set; }
}