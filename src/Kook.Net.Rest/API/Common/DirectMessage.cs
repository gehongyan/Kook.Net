using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class DirectMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("embeds")]
    public required EmbedBase[] Embeds { get; set; }

    [JsonPropertyName("attachments")]
    [JsonConverter(typeof(SafeAttachmentConverter))]
    public Attachment? Attachment { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("update_at")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? UpdateAt { get; set; }

    [JsonPropertyName("reactions")]
    public required Reaction[] Reactions { get; set; }

    [JsonPropertyName("author_id")]
    public ulong AuthorId { get; set; }

    [JsonPropertyName("author")]
    public User? Author { get; set; }

    [JsonPropertyName("image_name")]
    public required string ImageName { get; set; }

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("mention_info")]
    public MentionInfo? MentionInfo { get; set; }
}
