using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class MessageDeleteEvent
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}
