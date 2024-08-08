using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildMemberUpdateEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("is_vip")]
    public bool? IsVip { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
