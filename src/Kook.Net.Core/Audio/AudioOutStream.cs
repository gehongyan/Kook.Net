namespace Kook.Audio;

/// <summary>
///     表示一个通用的输出音频流。
/// </summary>
public abstract class AudioOutStream : AudioStream
{
    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void SetLength(long value) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
}
