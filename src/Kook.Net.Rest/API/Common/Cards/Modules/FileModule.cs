using System.Text.Json.Serialization;

namespace Kook.API;

internal class FileModule : ModuleBase
{
    [JsonPropertyName("src")]
    public required string Source { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("canDownload")]
    public bool? CanDownload { get; set; }
}
