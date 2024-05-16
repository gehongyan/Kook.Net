using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class EmbedsAppendEvent
{
    [JsonPropertyName("rong_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("embeds")]
    public required EmbedBase[] Embeds { get; set; }
}
