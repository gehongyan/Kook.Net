using System.Text.Json.Serialization;

namespace Kook.API;

internal class MentionInfo
{
    [JsonPropertyName("mention_part")]
    public MentionedUser[] MentionedUsers { get; set; }
    [JsonPropertyName("mention_role_part")]
    public Role[] MentionedRoles { get; set; }
    [JsonPropertyName("channel_part")]
    public MentionedChannel[] MentionedChannels { get; set; }
    [JsonPropertyName("item_part")]
    public Poke[] Pokes { get; set; }
}
