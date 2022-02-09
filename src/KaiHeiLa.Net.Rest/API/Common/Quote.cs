using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Quote
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("type")] public MessageType Type { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreateAt { get; set; }

    [JsonPropertyName("author")] public User Author { get; set; }
}