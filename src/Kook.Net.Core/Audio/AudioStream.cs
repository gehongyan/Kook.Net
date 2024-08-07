namespace Kook.Audio;

/// <summary>
///     表示一个通用的音频流。
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
    ///     向流中写入一个头部信息。
    /// </summary>
    /// <param name="seq"> 头部的序列号。 </param>
    /// <param name="timestamp"> 头部的时间戳。 </param>
    /// <param name="missed"> 头部是否用于标识丢失的数据包。 </param>
    /// <exception cref="InvalidOperationException"> 该流不接受头部信息。 </exception>
    public virtual void WriteHeader(ushort seq, uint timestamp, bool missed) =>
        throw new InvalidOperationException("This stream does not accept headers.");

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
    }

    /// <inheritdoc />
    public override void Flush()
    {
    }

    /// <summary>
    ///     清空该流.
    /// </summary>
    /// <remarks>
    ///     此操作会导致丢失所有缓冲的数据。
    /// </remarks>
    public void Clear()
    {
    }

    /// <summary>
    ///     清空该流.
    /// </summary>
    /// <param name="cancellationToken"> 用于取消该异步操作的取消令牌。 </param>
    /// <returns> 一个清空操作的异步任务。 </returns>
    /// <remarks>
    ///     此操作会导致丢失所有缓冲的数据。
    /// </remarks>
    public virtual Task ClearAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持获取此音频流的长度。 </exception>
    public override long Length =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持获取或设置此音频流的位置。 </exception>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持读取此音频流。 </exception>
    public override int Read(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持为此音频流设置长度。 </exception>
    public override void SetLength(long value) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在此音频流中进行定位操作。 </exception>
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
}
