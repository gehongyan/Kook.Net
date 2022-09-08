using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageAnimationPokeResource : PokeResourceBase
{
    [JsonPropertyName("preview_expired")]
    public string PreviewExpired { get; set; }
    [JsonPropertyName("webp")]
    public string WebP { get; set; }
    [JsonPropertyName("pag")]
    public string PAG { get; set; }
    [JsonPropertyName("gif")]
    public string GIF { get; set; }
    [JsonPropertyName("time")]
    public int Duration { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }
    [JsonPropertyName("percent")]
    public int Percent { get; set; }
}