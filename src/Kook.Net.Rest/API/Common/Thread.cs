using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.API;

internal class Thread
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("status")]
    public ThreadAuditStatus Status { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("post_id")]
    public ulong PostId { get; set; }

    [JsonPropertyName("medias")]
    public required ThreadMedia[] Medias { get; set; }

    [JsonPropertyName("preview_content")]
    public required string PreviewContent { get; set; }

    [JsonPropertyName("user")]
    public required GuildMember User { get; set; }

    [JsonPropertyName("category")]
    public ThreadCategory? Category { get; set; }

    [JsonPropertyName("tags")]
    public required IEnumerable<ThreadTag> Tags { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("mention")]
    public ulong[]? MentionedUserIds { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("mention_part")]
    public MentionedUser[]? MentionedUsers { get; set; }

    [JsonPropertyName("mention_role_part")]
    public Role[]? MentionedRoles { get; set; }

    [JsonPropertyName("channel_part")]
    public MentionedChannel[]? MentionedChannels { get; set; }

    [JsonPropertyName("item_part")]
    public Poke[]? Pokes { get; set; }
}
