using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class LiveInfo
{
    [JsonPropertyName("in_live")]
    public bool InLive { get; set; }

    [JsonPropertyName("audience_count")]
    public int AudienceCount { get; set; }

    [JsonPropertyName("audience_limit")]
    public int AudienceLimit { get; set; }

    [JsonPropertyName("live_thumb")]
    public string? LiveThumb { get; set; }

    [JsonPropertyName("live_start_time")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? LiveStartTime { get; set; }

    [JsonPropertyName("resolution")]
    public int Resolution { get; set; }

    [JsonPropertyName("frame_rate")]
    public int FrameRate { get; set; }

    [JsonPropertyName("tag")]
    public required string Tag { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(HexAlphaColorConverter))]
    public AlphaColor Color { get; set; }

    [JsonPropertyName("img_url")]
    public required string ImgUrl { get; set; }

    [JsonPropertyName("mode")]
    public int Mode { get; set; }
}
