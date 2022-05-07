using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class UnpinnedMessageEvent
{
    
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("operator_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong OperatorUserId { get; set; }
    
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}