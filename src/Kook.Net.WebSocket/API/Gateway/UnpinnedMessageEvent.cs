using System.Text.Json.Serialization;
using Kook.Net.Converters;

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