using System.Security.Principal;
using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class GuildEvent
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    
    [JsonPropertyName("notify_type")]
    public NotifyType NotifyType { get; set; }

    [JsonPropertyName("region")] 
    public string Region { get; set; }
    
    [JsonPropertyName("enable_open")] 
    public int EnableOpen { get; set; }
    
    [JsonPropertyName("open_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint OpenId { get; set; }
    
    [JsonPropertyName("default_channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong DefaultChannelId { get; set; }

    [JsonPropertyName("welcome_channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong WelcomeChannelId { get; set; }

    [JsonPropertyName("banner")] 
    public string Banner { get; set; }
    
    [JsonPropertyName("banner_status")] 
    public int BannerStatus { get; set; }
    
    [JsonPropertyName("custom_id")] 
    public string CustomId { get; set; }
    
    [JsonPropertyName("boost_num")]
    public int BoostNumber { get; set; }
    
    [JsonPropertyName("buffer_boost_num")]
    public int BufferBoostNumber { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }
}