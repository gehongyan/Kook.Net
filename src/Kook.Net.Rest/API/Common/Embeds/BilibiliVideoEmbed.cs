using System.Text.Json.Serialization;

namespace Kook.API;

internal class BilibiliVideoEmbed : EmbedBase
{
    [JsonPropertyName("origin_url")]
    public required string OriginUrl { get; set; }

    [JsonPropertyName("av_no")]
    public required string BvNumber { get; set; }

    [JsonPropertyName("iframe_path")]
    public required string IframePath { get; set; }

    [JsonPropertyName("duration")]
    public required int Duration { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("pic")]
    public required string Cover { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
