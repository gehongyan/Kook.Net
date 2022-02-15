using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ModifyMessageParams
{
    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("quote")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(QuoteMessageIdConverter))]
    public Guid? QuotedMessageId { get; set; }

    [JsonPropertyName("temp_target_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? EphemeralUserId { get; set; }
}