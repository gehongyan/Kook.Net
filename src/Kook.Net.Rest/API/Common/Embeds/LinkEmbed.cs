using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class LinkEmbed : EmbedBase
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("site_name")]
    public required string SiteName { get; set; }

    [JsonPropertyName("theme_color")]
    [JsonConverter(typeof(HexColorConverter))]
    public required Color Color { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
