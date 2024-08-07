namespace Kook.Audio.Streams;

///<summary> Converts Opus to PCM </summary>
public class OpusDecodeStream : AudioOutStream
{
    /// <summary>
    ///     Gets the sample rate.
    /// </summary>
    public const int SampleRate = OpusEncodeStream.SampleRate;

    private readonly AudioStream _next;
    private readonly OpusDecoder _decoder;
    private readonly byte[] _buffer;
    private bool _nextMissed;
    private bool _hasHeader;

    /// <inheritdoc />
    public OpusDecodeStream(AudioStream next)
    {
        _next = next;
        _buffer = new byte[OpusConverter.FrameBytes];
        _decoder = new OpusDecoder();
    }

    /// <exception cref="InvalidOperationException">Header received with no payload.</exception>
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        if (_hasHeader)
            throw new InvalidOperationException("Header received with no payload.");
        _hasHeader = true;

        _nextMissed = missed;
        _next.WriteHeader(seq, timestamp, missed);
    }

    /// <exception cref="InvalidOperationException">Received payload without an RTP header.</exception>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (!_hasHeader)
            throw new InvalidOperationException("Received payload without an RTP header.");
        _hasHeader = false;

        count = !_nextMissed || count > 0
            ? _decoder.DecodeFrame(buffer, offset, count, _buffer, 0, false)
            : _decoder.DecodeFrame(null, 0, 0, _buffer, 0, false);

        return _next.WriteAsync(_buffer, 0, count, cancellationToken);
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken)
        => _next.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken)
        => _next.ClearAsync(cancellationToken);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _decoder.Dispose();
            _next.Dispose();
        }
        base.Dispose(disposing);
    }
}
