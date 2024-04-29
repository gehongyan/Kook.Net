using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageEmbed : EmbedBase
{
    [JsonPropertyName("origin_url")]
    public required string OriginUrl { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
