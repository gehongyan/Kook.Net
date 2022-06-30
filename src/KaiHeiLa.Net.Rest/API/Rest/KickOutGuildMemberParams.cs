using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class KickOutGuildMemberParams
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }
}