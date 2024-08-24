namespace Kook.Audio.Streams;

/// <summary>
///     表示一个 RTP 帧读取流。
/// </summary>
public class RtpReadStream : AudioOutStream
{
    private readonly AudioStream _next;
    private readonly byte[] _buffer, _nonce;


    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public RtpReadStream(AudioStream next, int bufferSize = 4000)
    {
        _next = next;
        _buffer = new byte[bufferSize];
        _nonce = new byte[24];
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int headerSize = GetHeaderSize(buffer, offset);

        ushort seq = (ushort)((buffer[offset + 2] << 8) |
            (buffer[offset + 3] << 0));

        uint timestamp = (uint)((buffer[offset + 4] << 24) |
            (buffer[offset + 5] << 16) |
            (buffer[offset + 6] << 8) |
            (buffer[offset + 7] << 0));

        _next.WriteHeader(seq, timestamp, false);
        return _next.WriteAsync(buffer, offset + headerSize, count - headerSize, cancellationToken);
    }

    /// <summary>
    ///     尝试从 RTP 数据报中读取 SSRC。
    /// </summary>
    /// <param name="buffer"> 要从中读取 SSRC 的数据报。 </param>
    /// <param name="offset"> 数据报的偏移量。 </param>
    /// <param name="ssrc"> 如果读取成功，则为 SSRC 值；否则为 0。 </param>
    /// <returns> 读取是否成功。 </returns>
    public static bool TryReadSsrc(byte[] buffer, int offset, out uint ssrc)
    {
        ssrc = 0;
        if (buffer.Length - offset < 12)
            return false;

        int version = (buffer[offset + 0] & 0b1100_0000) >> 6;
        if (version != 2)
            return false;
        int type = (buffer[offset + 1] & 0b01111_1111);
        if (type != 100)
            return false;

        ssrc = (uint)((buffer[offset + 8] << 24) |
            (buffer[offset + 9] << 16) |
            (buffer[offset + 10] << 8) |
            (buffer[offset + 11] << 0));
        return true;
    }

    /// <summary>
    ///     获取 RTP 数据报的头部大小。
    /// </summary>
    /// <param name="buffer"> 要获取头部大小的数据报。 </param>
    /// <param name="offset"> 数据报的偏移量。 </param>
    /// <returns> 数据报的头部大小。 </returns>
    public static int GetHeaderSize(byte[] buffer, int offset)
    {
        byte headerByte = buffer[offset];
        bool extension = (headerByte & 0b0001_0000) != 0;
        int csics = (headerByte & 0b0000_1111) >> 4;

        if (!extension)
            return 12 + csics * 4;

        int extensionOffset = offset + 12 + (csics * 4);
        int extensionLength =
            (buffer[extensionOffset + 2] << 8) |
            (buffer[extensionOffset + 3]);
        return extensionOffset + 4 + (extensionLength * 4);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _next.Dispose();
        base.Dispose(disposing);
    }
}
