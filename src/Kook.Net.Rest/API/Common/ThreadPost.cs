using System.Text.Json.Serialization;

namespace Kook.API;

internal class ThreadPost
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("reply_id")]
    public ulong ReplyId { get; set; }

    [JsonPropertyName("thread_id")]
    public ulong ThreadId { get; set; }

    [JsonPropertyName("is_updated")]
    public bool IsUpdated { get; set; }

    [JsonPropertyName("mention")]
    public required ulong[] MentionedUserIds { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("mention_part")]
    public required MentionedUser[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_role_part")]
    public required Role[] MentionedRoles { get; set; }

    [JsonPropertyName("channel_part")]
    public required MentionedChannel[] MentionedChannels { get; set; }

    [JsonPropertyName("item_part")]
    public required Poke[] Pokes { get; set; }
}
