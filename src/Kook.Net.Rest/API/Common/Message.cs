using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Message
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("mention")]
    public required ulong[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionedAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public required uint[] MentionedRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionedHere { get; set; }

    [JsonPropertyName("embeds")]
    public required EmbedBase[] Embeds { get; set; }

    [JsonPropertyName("attachments")]
    [JsonConverter(typeof(SafeAttachmentConverter))]
    public Attachment? Attachment { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? UpdateAt { get; set; }

    [JsonPropertyName("reactions")]
    public required Reaction[] Reactions { get; set; }

    [JsonPropertyName("author")]
    public required User Author { get; set; }

    [JsonPropertyName("image_name")]
    public required string ImageName { get; set; }

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("mention_info")]
    public GuildMentionInfo? MentionInfo { get; set; }

    [JsonPropertyName("editable")]
    public bool? Editable { get; set; }
}
