using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageElement : ElementBase
{
    [JsonPropertyName("src")]
    public required string Source { get; set; }

    [JsonPropertyName("alt")]
    public string? Alternative { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(ImageSizeConverter))]
    public ImageSize? Size { get; set; }

    [JsonPropertyName("circle")]
    public bool? Circle { get; set; }
}
