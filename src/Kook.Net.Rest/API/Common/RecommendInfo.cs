using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class RecommendInfo
{
    [JsonPropertyName("guild_id")] 
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("open_id")]
    public uint OpenId { get; set; }
    
    [JsonPropertyName("default_channel_id")]
    public ulong DefaultChannelId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    
    [JsonPropertyName("banner")] 
    public string Banner { get; set; }
    
    [JsonPropertyName("desc")] 
    public string Description { get; set; }
    
    [JsonPropertyName("status")] 
    public int Status { get; set; }
    
    [JsonPropertyName("tag")] 
    public string Tag { get; set; }
    
    [JsonPropertyName("features")]
    public object[] Features { get; set; }
    
    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }
    
    [JsonPropertyName("custom_id")] 
    public string CustomId { get; set; }
    
    [JsonPropertyName("is_official_partner")]
    public int IsOfficialPartner { get; set; }

    [JsonPropertyName("sort")] 
    public int Sort { get; set; }
    
    [JsonPropertyName("audit_status")] 
    public int AuditStatus { get; set; }
    
    [JsonPropertyName("update_day_gap")]

    public int UpdateDayInterval { get; set; }
}