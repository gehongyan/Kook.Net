using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class AddReactionParams
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public string EmojiId { get; set; }
}
