using System.Text.Json.Serialization;

namespace Kook.API;

internal class Attachment
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("file_type")] public string FileType { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("duration")] public double? Duration { get; set; }
    [JsonPropertyName("width")] public int? Width { get; set; }
    [JsonPropertyName("height")] public int? Height { get; set; }
}
