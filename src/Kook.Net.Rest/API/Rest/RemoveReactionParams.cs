using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RemoveReactionParams
{
    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public string EmojiId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    public ulong? UserId { get; set; }
}
