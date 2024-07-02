namespace Kook.Audio;

/// <summary>
///     表示一个通用的 Opus 转换器。
/// </summary>
internal abstract class OpusConverter : IDisposable
{
    protected IntPtr _ptr;

    /// <summary>
    ///     获取音频的采样率，通常为 48000。
    /// </summary>
    public const int SamplingRate = 48000;

    /// <summary>
    ///     获取音频的通道数，通常为 2。
    /// </summary>
    public const int Channels = 2;

    /// <summary>
    ///     获取每帧的毫秒数，通常为 20。
    /// </summary>
    public const int FrameMillis = 20;

    /// <summary>
    ///     获取音频的样本大小，单位为字节，通常为 4。
    /// </summary>
    public const int SampleBytes = sizeof(short) * Channels;

    /// <summary>
    ///     获取每帧每个通道的样本数，通常为 960。
    /// </summary>
    public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;

    /// <summary>
    ///     获取每帧的样本数，通常为 1920。
    /// </summary>
    public const int FrameSamples = FrameSamplesPerChannel * Channels;

    /// <summary>
    ///     获取每帧的字节数，通常为 3840。
    /// </summary>
    public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;

    protected bool IsDisposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
            IsDisposed = true;
    }

    ~OpusConverter()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected static void CheckError(int result)
    {
        if (result < 0)
            throw new InvalidDataException($"Opus Error: {(OpusError)result}");
    }

    protected static void CheckError(OpusError error)
    {
        if ((int)error < 0)
            throw new InvalidDataException($"Opus Error: {error}");
    }
}
