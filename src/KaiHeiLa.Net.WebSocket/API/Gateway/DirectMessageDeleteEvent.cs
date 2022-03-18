using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class DirectMessageDeleteEvent
{
    [JsonPropertyName("author_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint AuthorId { get; set; }

    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("msg_id")] 
    public Guid MessageId { get; set; }
    
    [JsonPropertyName("deleted_at")] 
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset DeletedAt { get; set; }
    
    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }
}