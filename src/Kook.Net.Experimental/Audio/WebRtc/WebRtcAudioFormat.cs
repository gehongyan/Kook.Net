namespace Kook.Audio.WebRtc;

/// <summary>
///     WebRTC 音频客户端使用的音频格式常量。
/// </summary>
/// <remarks>
///     PCM 的格式与 <see cref="Kook.Audio.IAudioClient.CreatePcmStream(Kook.Audio.AudioApplication,System.Nullable{System.Int32},System.Int32,System.Int32)"/>
///     所约定的一致：48 kHz、16 位、双声道交错。每帧的时长与服务端通过 <c>rtpConfig.audio.ptime</c> 下发的 60 毫秒保持一致。
/// </remarks>
internal static class WebRtcAudioFormat
{
    /// <summary> 音频的采样率，同时也是 RTP 时钟频率。 </summary>
    public const int SamplingRate = 48000;

    /// <summary> 音频的通道数。 </summary>
    public const int Channels = 2;

    /// <summary> 每帧的毫秒数。 </summary>
    public const int FrameMillis = 60;

    /// <summary> 每个样本的字节数，即所有通道各一个 16 位采样。 </summary>
    public const int SampleBytes = sizeof(short) * Channels;

    /// <summary> 每帧每个通道的样本数，同时也是每帧的 RTP 时间戳增量。 </summary>
    public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;

    /// <summary> 每帧的字节数。 </summary>
    public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;

    /// <summary> Opus 单个包所能包含的最长音频（120 毫秒）所对应的每通道样本数。 </summary>
    public const int MaxDecodedSamplesPerChannel = SamplingRate / 1000 * 120;

    /// <summary> Opus 单个包的最大字节数。 </summary>
    public const int MaxPacketBytes = 1275;
}
