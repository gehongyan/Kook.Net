using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Quote
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("type")] public int Type { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }
    [JsonPropertyName("create_at")] public int CreateAt { get; set; }
    [JsonPropertyName("author")] public User Author { get; set; }
}