using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Attachment
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
}