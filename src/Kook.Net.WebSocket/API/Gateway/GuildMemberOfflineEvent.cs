using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildMemberOfflineEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("event_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset OfflineAt { get; set; }

    [JsonPropertyName("guilds")]
    public ulong[] CommonGuilds { get; set; }
}