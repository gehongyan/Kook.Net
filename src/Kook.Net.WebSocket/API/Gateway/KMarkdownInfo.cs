using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class KMarkdownInfo
{
    [JsonPropertyName("raw_content")]
    public required string RawContent { get; set; }

    [JsonPropertyName("mention")]
    public required ulong[] MentionedUserIds { get; set; }

    [JsonPropertyName("mention_part")]
    public required MentionedUser[] MentionedUsers { get; set; }

    [JsonPropertyName("item_part")]
    public required Poke[] Pokes { get; set; }
}
