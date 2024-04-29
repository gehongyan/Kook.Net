using System.Text.Json.Serialization;

namespace Kook.API;

internal class GuildMentionInfo : MentionInfo
{
    [JsonPropertyName("mention_role_part")]
    public required Role[] MentionedRoles { get; set; }
}
