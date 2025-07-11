using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ExtendedThreadPost : ThreadPost
{
    [JsonPropertyName("belong_to_post_id")]
    public ulong BelongToPostId { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateTime { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("replies")]
    public ExtendedThreadPost[]? Replies { get; set; }
}
