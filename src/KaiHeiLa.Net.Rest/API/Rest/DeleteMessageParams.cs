using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteMessageParams
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
    
    public static implicit operator DeleteMessageParams(Guid messageId) => new() {MessageId = messageId};
}