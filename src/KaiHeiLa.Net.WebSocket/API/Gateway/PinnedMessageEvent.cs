using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class PinnedMessageEvent
{
    
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("operator_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong OperatorUserId { get; set; }
    
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}