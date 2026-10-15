using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     <c>startAccompaniment</c> 通知中 <c>data</c> 的内容：正在播放的伴奏的信息。
/// </summary>
internal class VoiceAccompanimentInfo
{
    [JsonPropertyName("music_name")]
    public string? MusicName { get; set; }

    [JsonPropertyName("music_software")]
    public string? MusicSoftware { get; set; }

    [JsonPropertyName("singer")]
    public string? Singer { get; set; }
}
