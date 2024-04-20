using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class LinkEmbed : EmbedBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("site_name")]
    public string SiteName { get; set; }

    [JsonPropertyName("theme_color")]
    [JsonConverter(typeof(HexColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
