namespace Kook.Audio;

/// <summary>
///     Represents a generic incoming audio stream.
/// </summary>
public abstract class AudioInStream : AudioStream
{
    /// <summary>
    ///     Gets how many frames are available.
    /// </summary>
    public abstract int AvailableFrames { get; }

    /// <inheritdoc />
    public override bool CanRead => true;
    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <summary>
    ///     Read an RTP frame.
    /// </summary>
    public abstract Task<RtpFrame> ReadFrameAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Try reading an RTP frame.
    /// </summary>
    public abstract bool TryReadFrame(CancellationToken cancellationToken, out RtpFrame frame);

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) => throw new NotSupportedException();
}
