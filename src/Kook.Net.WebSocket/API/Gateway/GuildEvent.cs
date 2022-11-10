using System.Security.Principal;
using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildEvent
{
    [JsonPropertyName("id")]
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    
    [JsonPropertyName("notify_type")]
    public NotifyType NotifyType { get; set; }

    [JsonPropertyName("region")] 
    public string Region { get; set; }
    
    [JsonPropertyName("enable_open")] 
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool EnableOpen { get; set; }
    
    [JsonPropertyName("open_id")]
    public uint? OpenId { get; set; }
    
    [JsonPropertyName("default_channel_id")]
    public ulong DefaultChannelId { get; set; }

    [JsonPropertyName("welcome_channel_id")]
    public ulong WelcomeChannelId { get; set; }

    [JsonPropertyName("banner")] 
    public string Banner { get; set; }
    
    [JsonPropertyName("banner_status")] 
    public int BannerStatus { get; set; }
    
    [JsonPropertyName("custom_id")] 
    public string CustomId { get; set; }
    
    [JsonPropertyName("boost_num")]
    public int BoostSubscriptionCount { get; set; }
    
    [JsonPropertyName("buffer_boost_num")]
    public int BufferBoostSubscriptionCount { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }
}