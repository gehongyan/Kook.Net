using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class UnpinnedMessageEvent
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("operator_id")]
    public ulong OperatorUserId { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}
