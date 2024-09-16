namespace Kook.Audio.Streams;

///<summary>
///     表示一个 Opus 解码音频流。
/// </summary>
public class OpusDecodeStream : AudioOutStream
{
    /// <inheritdoc cref="Kook.Audio.Streams.OpusEncodeStream.SampleRate" />
    public const int SampleRate = OpusEncodeStream.SampleRate;

    private readonly AudioStream _next;
    private readonly OpusDecoder _decoder;
    private readonly byte[] _buffer;
    private bool _nextMissed;
    private bool _hasHeader;

    /// <summary>
    ///     初始化一个 <see cref="OpusDecodeStream"/> 类的新实例。
    /// </summary>
    public OpusDecodeStream(AudioStream next)
    {
        _next = next;
        _buffer = new byte[OpusConverter.FrameBytes];
        _decoder = new OpusDecoder();
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> 该流接收到没有有效负载的 RTP 头部。 </exception>
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        if (_hasHeader)
        {
            _hasHeader = false;
            throw new InvalidOperationException("Header received with no payload");
        }

        _hasHeader = true;

        _nextMissed = missed;
        _next.WriteHeader(seq, timestamp, missed);
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> 该流接收到没有 RTP 头部的有效负载。 </exception>
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
