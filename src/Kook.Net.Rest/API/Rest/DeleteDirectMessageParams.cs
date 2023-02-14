using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteDirectMessageParams
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    public static implicit operator DeleteDirectMessageParams(Guid messageId) => new() { MessageId = messageId };
}
