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
    public string Content { get; set; }

    [JsonPropertyName("mention")]
    public ulong[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionedAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionedRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionedHere { get; set; }

    [JsonPropertyName("embeds")]
    public EmbedBase[] Embeds { get; set; }

    [JsonPropertyName("attachment")]
    public Attachment Attachment { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdateAt { get; set; }

    [JsonPropertyName("reactions")]
    public Reaction[] Reactions { get; set; }

    [JsonPropertyName("author")]
    public User Author { get; set; }

    [JsonPropertyName("image_name")]
    public string ImageName { get; set; }

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }

    [JsonPropertyName("mention_info")]
    public MentionInfo MentionInfo { get; set; }
}
