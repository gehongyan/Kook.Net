using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class ImageEmbed : EmbedBase
{
    [JsonPropertyName("origin_url")]
    public string OriginUrl { get; set; }
}