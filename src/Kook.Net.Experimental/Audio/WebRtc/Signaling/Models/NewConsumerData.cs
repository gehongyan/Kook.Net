using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class NewConsumerData
{
    [JsonPropertyName("peerId")]
    public required string PeerId { get; set; }

    [JsonPropertyName("producerId")]
    public required string ProducerId { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("kind")]
    public required string Kind { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    // 服务端为该 Consumer 生成的 RTP 参数（含下行 SSRC、PT、头扩展），信令层不解析，原样透传给 WebRTC 层
    [JsonPropertyName("rtpParameters")]
    public required JsonElement RtpParameters { get; set; }

    [JsonPropertyName("appData")]
    public VoiceConsumerAppData? AppData { get; set; }

    [JsonPropertyName("producerPaused")]
    public bool? ProducerPaused { get; set; }
}
