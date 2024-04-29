using System.Text.Json;
using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewayEvent<T>
{
    [JsonPropertyName("channel_type")]
    public required string ChannelType { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("target_id")]
    [JsonConverter(typeof(SafeUInt64Converter))]
    public ulong TargetId { get; set; }

    [JsonPropertyName("author_id")]
    public uint AuthorId { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("extra")]
    public required T ExtraData { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("msg_timestamp")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset MessageTimestamp { get; set; }
}
