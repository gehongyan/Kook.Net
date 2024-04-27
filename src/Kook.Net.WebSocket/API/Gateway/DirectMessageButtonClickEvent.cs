using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class DirectMessageButtonClickEvent
{
    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("channel_type")]
    public required string ChannelType { get; set; }

    [JsonPropertyName("user_info")]
    public required User User { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }
}
