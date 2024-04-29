using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildEvent
{
    [JsonPropertyName("id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; } // TODO: 服务器拥有者更新

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("notify_type")]
    public NotifyType NotifyType { get; set; }

    [JsonPropertyName("region")]
    public required string Region { get; set; }

    [JsonPropertyName("enable_open")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool EnableOpen { get; set; }

    [JsonPropertyName("open_id")]
    [JsonConverter(typeof(NullableUInt32Converter))]
    public uint? OpenId { get; set; }

    [JsonPropertyName("default_channel_id")]
    public ulong DefaultChannelId { get; set; }

    [JsonPropertyName("welcome_channel_id")]
    public ulong WelcomeChannelId { get; set; }

    [JsonPropertyName("banner")]
    public required string Banner { get; set; } // TODO: 服务器横幅更新

    [JsonPropertyName("banner_status")]
    public int BannerStatus { get; set; }

    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("boost_num")]
    public int BoostSubscriptionCount { get; set; }

    [JsonPropertyName("buffer_boost_num")]
    public int BufferBoostSubscriptionCount { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("auto_delete_time")]
    public string? AutoDeleteTime { get; set; }
}
