using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class PrivateReaction
{
    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }

    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("emoji")] public Emoji Emoji { get; set; }
}