using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class Quote
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("rong_id")] public Guid QuotedMessageId { get; set; }
    [JsonPropertyName("type")] public MessageType Type { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("author")] public User Author { get; set; }
}