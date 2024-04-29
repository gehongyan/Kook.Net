using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Invite
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("guild_name")]
    public required string GuildName { get; set; }

    [JsonPropertyName("channel_name")]
    public string? ChannelName { get; set; }

    [JsonPropertyName("type")]
    public ChannelType ChannelType { get; set; }

    [JsonPropertyName("url_code")]
    public required string UrlCode { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("user")]
    public required User Inviter { get; set; }

    [JsonPropertyName("expire_time")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("remaining_times")]
    public int RemainingTimes { get; set; }

    [JsonPropertyName("using_times")]
    public int UsingTimes { get; set; }

    [JsonPropertyName("duration")]
    [JsonConverter(typeof(NullableTimeSpanConverter))]
    public TimeSpan? Duration { get; set; }

    [JsonPropertyName("invitees_count")]
    public int InviteesCount { get; set; }

    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreatedAt { get; set; }
}
