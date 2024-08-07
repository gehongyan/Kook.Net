namespace Kook.Audio;

/// <summary>
///     Represents a generic Opus converter.
/// </summary>
internal abstract class OpusConverter : IDisposable
{
    protected IntPtr _ptr;

    /// <summary>
    ///     Gets the sampling rate of the audio, typically 48000.
    /// </summary>
    public const int SamplingRate = 48000;

    /// <summary>
    ///     Gets the number of channels of the audio, typically 2.
    /// </summary>
    public const int Channels = 2;

    /// <summary>
    ///     Gets the milliseconds per frame, typically 40.
    /// </summary>
    public const int FrameMillis = 40;

    /// <summary>
    ///     Gets the sample size, in bytes, of the audio, typically 4.
    /// </summary>
    public const int SampleBytes = sizeof(short) * Channels;

    /// <summary>
    ///     Gets the number of samples per channel per frame, typically 1920.
    /// </summary>
    public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;

    /// <summary>
    ///     Gets the number of samples per frame, typically 3840.
    /// </summary>
    public const int FrameSamples = FrameSamplesPerChannel * Channels;

    /// <summary>
    ///     Gets the number of bytes per frame, typically 7680.
    /// </summary>
    public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;

    protected bool IsDisposed;

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
