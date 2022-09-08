using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class KMarkdownInfo
{
    [JsonPropertyName("raw_content")]
    public string RawContent { get; set; }
    
    [JsonPropertyName("mention")]
    public ulong[] MentionedUserIds { get; set; }
    [JsonPropertyName("mention_part")]
    public MentionedUser[] MentionedUsers { get; set; }
    [JsonPropertyName("item_part")]
    public Poke[] Pokes { get; set; }
}