using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class DirectMessageUpdateEvent
{
    [JsonPropertyName("author_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint AuthorId { get; set; }

    // self user id in fact
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("msg_id")] 
    public Guid MessageId { get; set; }
    
    [JsonPropertyName("content")] 
    public string Content { get; set; }
    
    [JsonPropertyName("updated_at")] 
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdatedAt { get; set; }
    
    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }
}