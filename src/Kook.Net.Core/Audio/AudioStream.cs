namespace Kook.Audio;

/// <summary>
///  Represents a generic audio stream.
/// </summary>
public abstract class AudioStream : Stream
{
    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <summary>
    ///     Writes a header to the stream.
    /// </summary>
    /// <param name="seq"> The sequence number of the header. </param>
    /// <param name="timestamp"> The timestamp of the header. </param>
    /// <param name="missed"> Whether the header is for a missed packet. </param>
    // <exception cref="InvalidOperationException"> This stream does not accept headers. </exception>
    public virtual void WriteHeader(ushort seq, uint timestamp, bool missed) =>
        throw new InvalidOperationException("This stream does not accept headers.");

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public override void Flush()
    {
        FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Clears the stream, causing all buffered data to be lost.
    /// </summary>
    public void Clear()
    {
        ClearAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Clears the stream, causing all buffered data to be lost.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token to be used. </param>
    /// <returns> A task that represents an asynchronous clear operation. </returns>
    public virtual Task ClearAsync(CancellationToken cancellationToken) => Task.Delay(0);

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Reading stream length is not supported.</exception>
    public override long Length =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Getting or setting this stream position is not supported.</exception>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Reading this stream is not supported.</exception>
    public override int Read(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Setting the length to this stream is not supported.</exception>
    public override void SetLength(long value) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Seeking this stream is not supported..</exception>
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
}
