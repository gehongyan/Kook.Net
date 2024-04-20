using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageAnimationPokeResource : PokeResourceBase
{
    [JsonPropertyName("preview_expired")]
    public required string PreviewExpired { get; set; }

    [JsonPropertyName("webp")]
    public required string WebP { get; set; }

    [JsonPropertyName("pag")]
    public required string PAG { get; set; }

    [JsonPropertyName("gif")]
    public required string GIF { get; set; }

    [JsonPropertyName("time")]
    public required int Duration { get; set; }

    [JsonPropertyName("width")]
    public required int Width { get; set; }

    [JsonPropertyName("height")]
    public required int Height { get; set; }

    [JsonPropertyName("percent")]
    public required int Percent { get; set; }
}
