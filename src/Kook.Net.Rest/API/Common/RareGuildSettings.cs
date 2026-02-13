using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class RareGuildSettings
{
    [JsonPropertyName("frame")]
    public required IReadOnlyDictionary<string, string> Frame { get; set; }

    [JsonPropertyName("frame_color")]
    [JsonConverter(typeof(RawValueColorConverter))]
    public required Color FrameColor { get; set; }

    [JsonPropertyName("rare_nameplate")]
    public required IReadOnlyDictionary<string, string> RareNameplate { get; set; }

    [JsonPropertyName("cover_image")]
    public required string CoverImage { get; set; }

    [JsonPropertyName("brief_text")]
    public required string BriefText { get; set; }

    [JsonPropertyName("voice_channel_link")]
    public string? VoiceChannelLink { get; set; }

    [JsonPropertyName("desc")]
    public required string Description { get; set; }

    [JsonPropertyName("id_icon")]
    public required string IdIcon { get; set; }
}
