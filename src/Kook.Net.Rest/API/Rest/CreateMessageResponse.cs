using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateMessageResponse
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("msg_timestamp")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset MessageTimestamp { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
