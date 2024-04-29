using System.Text.Json.Serialization;

namespace Kook.API;

internal class MentionInfo
{
    [JsonPropertyName("mention_part")]
    public required MentionedUser[] MentionedUsers { get; set; }

    [JsonPropertyName("channel_part")]
    public required MentionedChannel[] MentionedChannels { get; set; }

    [JsonPropertyName("item_part")]
    public required Poke[] Pokes { get; set; }
}
