using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Quote
{
    [JsonPropertyName("id")]
    [JsonConverter(typeof(NullableGuidConverter))]
    public Guid? QuotedMessageId { get; set; }

    // TODO: To be investigated
    [JsonPropertyName("rong_id")]
    public Guid? RongId { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("author")]
    public required User Author { get; set; }

    [JsonPropertyName("can_jump")]
    public bool CanJump { get; set; }
}
