using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class ChannelSortEvent
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channels")]
    public required ChannelSortCategory[] Channels { get; set; }
}

internal class ChannelSortCategory
{
    [JsonPropertyName("id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("channels")]
    public required ulong[] Channels { get; set; }
}
