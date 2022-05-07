using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Invite
{
    [JsonPropertyName("id")] public uint Id { get; set; }

    [JsonPropertyName("channel_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("guild_name")] public string GuildName { get; set; }
    [JsonPropertyName("channel_name")] public string ChannelName { get; set; }
    [JsonPropertyName("type")] public ChannelType ChannelType { get; set; }
    [JsonPropertyName("url_code")] public string UrlCode { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("user")] public User Inviter { get; set; }
    
    [JsonPropertyName("expire_time")] 
    [JsonConverter(typeof(NullableDateTimeOffsetConverter))]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("remaining_times")]
    public int RemainingTimes { get; set; }

    [JsonPropertyName("using_times")] 
    public int UsingTimes { get; set; }

    [JsonPropertyName("duration")]
    [JsonConverter(typeof(NullableTimeSpanConverter))]
    public TimeSpan? Duration { get; set; }
}