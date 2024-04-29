using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class DirectMessageUpdateEvent
{
    [JsonPropertyName("author_id")]
    public ulong AuthorId { get; set; }

    // self user id in fact
    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("kmarkdown")]
    public MentionInfo? MentionInfo { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("attachments")]
    [JsonConverter(typeof(SafeAttachmentConverter))]
    public Attachment? Attachment { get; set; }

    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }
}
