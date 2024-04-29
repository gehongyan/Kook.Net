using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class AddReactionParams
{
    [JsonPropertyName("msg_id")]
    public required Guid MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public required string EmojiId { get; set; }
}
