using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class BoostSubscription
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("start_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeSecondsConverter))]
    public DateTimeOffset StartTime { get; set; }

    [JsonPropertyName("end_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeSecondsConverter))]
    public DateTimeOffset EndTime { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}
