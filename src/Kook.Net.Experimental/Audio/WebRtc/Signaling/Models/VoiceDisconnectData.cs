using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceDisconnectData
{
    // 实测：被管理员移出语音频道、同一账号用 WebRTC 连接了另一个频道、同一账号用官方 UDP 方案加入了同一个频道，errorCode 都是 41106，
    // errorReason 分别是不同的中文文案或空字符串
    [JsonPropertyName("errorCode")]
    public int? ErrorCode { get; set; }

    [JsonPropertyName("errorReason")]
    public string? ErrorReason { get; set; }
}
