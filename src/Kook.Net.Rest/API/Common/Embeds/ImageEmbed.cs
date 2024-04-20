using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageEmbed : EmbedBase
{
    [JsonPropertyName("origin_url")]
    public string OriginUrl { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
