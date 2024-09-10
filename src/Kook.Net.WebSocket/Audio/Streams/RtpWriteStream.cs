namespace Kook.Audio.Streams;

/// <summary>
///     表示一个 RTP 帧写入流。
/// </summary>
public class RtpWriteStream : AudioOutStream
{
    private readonly AudioStream _next;
    private readonly byte[] _header;
    private readonly byte[] _buffer;
    private ushort _nextSeq;
    private uint _nextTimestamp;
    private bool _hasHeader;

    /// <summary>
    ///     初始化一个 <see cref="RtpWriteStream"/> 类的新实例。
    /// </summary>
    /// <param name="next"> 要写入 RTP 帧数据的音频流，是音频流写入链中的下一个音频流对象。 </param>
    /// <param name="ssrc"> RTP 连接的同步源标识符。 </param>
    /// <param name="payloadType"> RTP 连接的负载类型。 </param>
    /// <param name="bufferSize"> RTP 帧缓冲区的大小，默认为 4000 字节。 </param>
    public RtpWriteStream(AudioStream next, uint ssrc, byte payloadType, int bufferSize = 4000)
    {
        if ((payloadType & 0x80) != 0)
            throw new ArgumentOutOfRangeException(nameof(payloadType), "Payload type must be less than 128");
        _next = next;
        _buffer = new byte[bufferSize];
        _header = new byte[24];
        // 10.. .... = Version: RFC 1889 Version (2)
        // ..0. .... = Padding: False
        // ...0 .... = Extension: False
        // .... 0000 = Contributing source identifiers count: 0
        _header[0] = 0x80;
        // 1... .... = Marker: True
        // .xxx xxxx = Payload type
        _header[1] = (byte)(0x80 | payloadType);
        // Synchronization Source identifier
        _header[8] = (byte)(ssrc >> 24);
        _header[9] = (byte)(ssrc >> 16);
        _header[10] = (byte)(ssrc >> 8);
        _header[11] = (byte)(ssrc >> 0);
    }

    /// <inheritdoc />
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        if (_hasHeader)
        {
            _hasHeader = false;
            throw new InvalidOperationException("Header received with no payload");
        }

        _hasHeader = true;
        _nextSeq = seq;
        _nextTimestamp = timestamp;
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_hasHeader)
            throw new InvalidOperationException("Received payload without an RTP header");
        _hasHeader = false;

        unchecked
        {
            // Sequence number
            _header[2] = (byte)(_nextSeq >> 8);
            _header[3] = (byte)(_nextSeq >> 0);
            // Timestamp
            _header[4] = (byte)(_nextTimestamp >> 24);
            _header[5] = (byte)(_nextTimestamp >> 16);
            _header[6] = (byte)(_nextTimestamp >> 8);
            _header[7] = (byte)(_nextTimestamp >> 0);
        }
        Buffer.BlockCopy(_header, 0, _buffer, 0, 12); //Copy RTP header from to the buffer
        Buffer.BlockCopy(buffer, offset, _buffer, 12, count);

        _next.WriteHeader(_nextSeq, _nextTimestamp, false);
        return _next.WriteAsync(_buffer, 0, count + 12, cancellationToken);
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _next.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken) =>
        _next.ClearAsync(cancellationToken);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _next.Dispose();
        base.Dispose(disposing);
    }
}
