using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildMemberOnlineEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("event_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset OnlineAt { get; set; }

    [JsonPropertyName("guilds")]
    public required ulong[] CommonGuilds { get; set; }
}
