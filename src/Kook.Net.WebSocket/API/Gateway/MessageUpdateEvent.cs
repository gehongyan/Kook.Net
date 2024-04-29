using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class MessageUpdateEvent
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("mention")]
    public required ulong[] Mention { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionedAll { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionedHere { get; set; }

    [JsonPropertyName("mention_roles")]
    public required uint[] MentionedRoles { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("kmarkdown")]
    public required MentionInfo MentionInfo { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("attachments")]
    [JsonConverter(typeof(SafeAttachmentConverter))]
    public Attachment? Attachment { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}
