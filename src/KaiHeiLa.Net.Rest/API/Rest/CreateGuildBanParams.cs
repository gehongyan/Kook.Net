using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateGuildBanParams
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("remark")]
    public string Reason { get; set; } 
    
    [JsonPropertyName("del_msg_days")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DeleteMessageDays { get; set; }
}