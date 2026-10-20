using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class QuickJoinParams
{
    [JsonPropertyName("transport")]
    public string Transport { get; set; } = "webrtc";

    [JsonPropertyName("forceTcp")]
    public bool ForceTcp { get; set; }

    [JsonPropertyName("forceRelay")]
    public bool ForceRelay { get; set; }

    [JsonPropertyName("useAvpf")]
    public bool? UseAvpf { get; set; }

    [JsonPropertyName("srtpPolicy")]
    public int? SrtpPolicy { get; set; }
}
