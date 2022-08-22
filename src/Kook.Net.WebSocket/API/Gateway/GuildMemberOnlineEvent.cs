using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildMemberOnlineEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("event_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset OnlineAt { get; set; }

    [JsonPropertyName("guilds")]
    public ulong[] CommonGuilds { get; set; }
}