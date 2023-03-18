using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyMessageParams
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("quote")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(NullableGuidConverter))]
    public Guid? QuotedMessageId { get; set; }

    [JsonPropertyName("temp_target_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? EphemeralUserId { get; set; }

    public ModifyMessageParams(Guid messageId, string content)
    {
        MessageId = messageId;
        Content = content;
    }
}
