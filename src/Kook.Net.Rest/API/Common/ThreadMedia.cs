using System.Text.Json.Serialization;

namespace Kook.API;

internal class ThreadMedia
{
    [JsonPropertyName("type")]
    public AttachmentType Type { get; set; }

    [JsonPropertyName("src")]
    public required string Source { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("cover")]
    public string? Cover { get; set; }
}
