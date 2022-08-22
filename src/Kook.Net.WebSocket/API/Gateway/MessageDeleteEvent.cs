using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class MessageDeleteEvent
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}