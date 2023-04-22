using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Guild
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("topic")]
    public string Topic { get; set; }

    [JsonPropertyName("user_id")]
    public ulong OwnerId { get; set; }

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
    [JsonConverter(typeof(NullableUInt32Converter))]
    public uint? OpenId { get; set; }

    [JsonPropertyName("default_channel_id")]
    public ulong DefaultChannelId { get; set; }

    [JsonPropertyName("default_channel_id_setting")]
    public ulong DefaultChannelIdSetting { get; set; }

    [JsonPropertyName("welcome_channel_id")]
    public ulong WelcomeChannelId { get; set; }

    [JsonPropertyName("roles")]
    public Role[] Roles { get; set; }

    [JsonPropertyName("channels")]
    public Channel[] Channels { get; set; }
}
