using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class Reaction
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("emoji")] public Emoji Emoji { get; set; }
}
