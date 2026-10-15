using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class InitSpeakingParams
{
    [JsonPropertyName("pauseHeadSet")]
    public bool PauseHeadSet { get; set; }

    [JsonPropertyName("pauseMic")]
    public bool PauseMic { get; set; }

    [JsonPropertyName("startAiReduce")]
    public bool StartAiReduce { get; set; }

    [JsonPropertyName("startAccompaniment")]
    public bool StartAccompaniment { get; set; }

    // mediasoup 生成的发送端 RTP 参数，信令层不解析，原样透传
    [JsonPropertyName("rtpParameters")]
    public required JsonElement RtpParameters { get; set; }

    [JsonPropertyName("pDtls")]
    public required VoiceDtlsParameters ProducerDtls { get; set; }

    [JsonPropertyName("cDtls")]
    public required VoiceDtlsParameters ConsumerDtls { get; set; }
}
