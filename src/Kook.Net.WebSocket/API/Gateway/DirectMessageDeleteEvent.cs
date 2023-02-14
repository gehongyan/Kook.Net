using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class DirectMessageDeleteEvent
{
    [JsonPropertyName("author_id")]
    public uint AuthorId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("deleted_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset DeletedAt { get; set; }

    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }
}
