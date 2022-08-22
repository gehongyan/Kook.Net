using System.Text.Json.Serialization;

namespace Kook.API;

internal class BilibiliVideoEmbed : EmbedBase
{
    [JsonPropertyName("origin_url")]
    public string OriginUrl { get; set; }
    
    [JsonPropertyName("av_no")]
    public string BvNumber { get; set; }
    
    [JsonPropertyName("iframe_path")]
    public string IframePath { get; set; }
    
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("pic")]
    public string Cover { get; set; }
}