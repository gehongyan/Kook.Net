using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Embed
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("origin_url")]
    public string OriginalUrl { get; set; }
    [JsonPropertyName("av_no")]
    public string AvNumber { get; set; }
    [JsonPropertyName("iframe_path")]
    public string IframePath { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("pic")]
    public string Picture { get; set; }
}