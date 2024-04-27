using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildMemberUpdateEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("nickname")]
    public required string Nickname { get; set; }
}
