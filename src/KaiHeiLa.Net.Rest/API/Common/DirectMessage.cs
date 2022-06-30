using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class DirectMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    [JsonPropertyName("embeds")]
    public EmbedBase[] Embeds { get; set; }

    [JsonPropertyName("attachment")]
    public Attachment Attachment { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateAt { get; set; }
    
    [JsonPropertyName("update_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdateAt { get; set; }

    [JsonPropertyName("reactions")]
    public Reaction[] Reactions { get; set; }

    [JsonPropertyName("author_id")]
    public ulong AuthorId { get; set; }

    [JsonPropertyName("image_name")]
    public string ImageName { get; set; }

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }
    
    [JsonPropertyName("mention_info")]
    public MentionInfo MentionInfo { get; set; }
}