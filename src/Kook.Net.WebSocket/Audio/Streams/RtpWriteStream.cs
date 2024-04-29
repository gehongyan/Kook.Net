namespace Kook.Audio.Streams;

///<summary> Wraps data in a RTP frame </summary>
public class RtpWriteStream : AudioOutStream
{
    private readonly AudioStream _next;
    private readonly byte[] _header;
    private readonly byte[] _buffer;
    private ushort _nextSeq;
    private uint _nextTimestamp;
    private bool _hasHeader;

    /// <summary>
    ///     Creates a new instance of <see cref="RtpWriteStream"/>.
    /// </summary>
    /// <param name="next"> The next stream in the chain. </param>
    /// <param name="ssrc"> The SSRC of the RTP connection. </param>
    /// <param name="bufferSize"> The buffer size to use. </param>
    public RtpWriteStream(AudioStream next, uint ssrc, int bufferSize = 4000)
    {
        _next = next;
        _buffer = new byte[bufferSize];
        _header = new byte[24];
        // 10.. .... = Version: RFC 1889 Version (2)
        // ..0. .... = Padding: False
        // ...0 .... = Extension: False
        // .... 0000 = Contributing source identifiers count: 0
        _header[0] = 0x80;
        // 1... .... = Marker: True
        // .110 0100 = Payload type: DynamicRTP-Type-100 (100)
        _header[1] = 0xe4;
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
            throw new InvalidOperationException("Header received with no payload");

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
