using System.Collections.Concurrent;

namespace Kook.Audio.Streams;

///<summary>
///     表示输入音频流，用于在 <see cref="Kook.Audio.IAudioClient"/> 实例中，从语音服务器接收语音数据。
/// </summary>
public class InputStream : AudioInStream
{
    private const int MaxFrames = 100; //1-2 Seconds

    private readonly ConcurrentQueue<RtpFrame> _frames;
    private readonly SemaphoreSlim _signal;
    private ushort _nextSeq;
    private uint _nextTimestamp;
    private bool _nextMissed;
    private bool _hasHeader;
    private bool _isDisposed;

    /// <inheritdoc />
    public override bool CanRead => !_isDisposed;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override int AvailableFrames => _signal.CurrentCount;

    /// <inheritdoc />
    public InputStream()
    {
        _frames = new ConcurrentQueue<RtpFrame>();
        _signal = new SemaphoreSlim(0, MaxFrames);
    }

    /// <inheritdoc />
    public override bool TryReadFrame(CancellationToken cancellationToken, out RtpFrame frame)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_signal.Wait(0))
        {
            _frames.TryDequeue(out frame);
            return true;
        }

        frame = default;
        return false;
    }

    /// <inheritdoc />
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        RtpFrame frame = await ReadFrameAsync(cancellationToken).ConfigureAwait(false);
        if (count < frame.Payload.Length)
            throw new InvalidOperationException("Buffer is too small.");
        Buffer.BlockCopy(frame.Payload, 0, buffer, offset, frame.Payload.Length);
        return frame.Payload.Length;
    }

    /// <inheritdoc />
    public override async Task<RtpFrame> ReadFrameAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
        _frames.TryDequeue(out RtpFrame frame);
        return frame;
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
        _nextMissed = missed;
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_signal.CurrentCount >= MaxFrames) //1-2 seconds
        {
            _hasHeader = false;
            return Task.CompletedTask; //Buffer overloaded
        }

        if (!_hasHeader)
            throw new InvalidOperationException("Received payload without an RTP header");
        _hasHeader = false;
        byte[] payload = new byte[count];
        Buffer.BlockCopy(buffer, offset, payload, 0, count);

        _frames.Enqueue(new RtpFrame(
            sequence: _nextSeq,
            timestamp: _nextTimestamp,
            missed: _nextMissed,
            payload: payload
        ));
        _signal.Release();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
                _signal?.Dispose();
            _isDisposed = true;
        }

        base.Dispose(disposing);
    }
}
