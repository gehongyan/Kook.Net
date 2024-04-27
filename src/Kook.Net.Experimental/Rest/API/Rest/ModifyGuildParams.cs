using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyGuildParams
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("region")]
    public string? RegionId { get; set; }

    [JsonPropertyName("default_channel_id")]
    public ulong? DefaultChannelId { get; set; }

    [JsonPropertyName("welcome_channel_id")]
    public ulong? WelcomeChannelId { get; set; }

    [JsonPropertyName("enable_open")]
    public bool? EnableOpen { get; set; }

    [JsonPropertyName("widget_invite_channel_id")]
    public ulong? WidgetChannelId { get; set; }
}
