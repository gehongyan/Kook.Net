using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyDirectMessageParams
{
    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("quote")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(NullableGuidConverter))]
    public Guid? QuotedMessageId { get; set; }

    public ModifyDirectMessageParams(Guid messageId, string content)
    {
        MessageId = messageId;
        Content = content;
    }
}
