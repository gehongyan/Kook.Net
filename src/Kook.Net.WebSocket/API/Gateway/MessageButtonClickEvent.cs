using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.API.Gateway;

internal class MessageButtonClickEvent
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
    public required GuildMember User { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }
}
