using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class CreateMessageResponse
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("msg_timestamp")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset MessageTimestamp { get; set; }

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; }
}