using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewayEvent
{
    [JsonPropertyName("channel_type")] public string ChannelType { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(MessageTypeConverter))]
    public MessageType Type { get; set; }

    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong TargetId { get; set; }

    [JsonPropertyName("author_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint AuthorId { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }
    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }

    [JsonPropertyName("msg_timestamp")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset MessageTimestamp { get; set; }

    [JsonPropertyName("nonce")] public string Nonce { get; set; }
    [JsonPropertyName("extra")] public object ExtraData { get; set; }
}