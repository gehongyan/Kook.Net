using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class UserChat
{
    [JsonPropertyName("code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid Code { get; set; }

    [JsonPropertyName("last_read_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LastReadTime { get; set; }

    [JsonPropertyName("latest_msg_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LatestMessageTime { get; set; }

    [JsonPropertyName("unread_count")]
    public int UnreadCount { get; set; }

    [JsonPropertyName("target_info")]
    public required User Recipient { get; set; }
}
