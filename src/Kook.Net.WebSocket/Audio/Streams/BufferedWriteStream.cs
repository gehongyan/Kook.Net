using Kook.Logging;
using System.Collections.Concurrent;

namespace Kook.Audio.Streams;

///<summary>
///     表示一个定时缓冲的音频写入流，用于将定时缓冲的音频数据写入到下一个音频流中。
/// </summary>
public class BufferedWriteStream : AudioOutStream
{
    private const int MaxSilenceFrames = 10;

    private struct Frame(byte[] buffer, int bytes)
    {
        public readonly byte[] Buffer = buffer;
        public readonly int Bytes = bytes;
    }

    private static readonly byte[] _silenceFrame = [];

    private readonly AudioClient _client;
    private readonly AudioStream _next;
    private readonly CancellationTokenSource _disposeTokenSource, _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentQueue<Frame> _queuedFrames;
    private readonly ConcurrentQueue<byte[]> _bufferPool;
    private readonly SemaphoreSlim _queueLock;
    private readonly Logger _logger;
    private readonly int _ticksPerFrame, _queueLength;
    private bool _isPreloaded;
    // private int _silenceFrames;

    internal BufferedWriteStream(AudioStream next, AudioClient client, int bufferMillis,
        CancellationToken cancellationToken, Logger logger, int maxFrameSize = 1500)
    {
        //maxFrameSize = 1275 was too limiting at 128kbps,2ch,60ms
        _next = next;
        _client = client;
        _ticksPerFrame = OpusConverter.FrameMillis;
        _logger = logger;
        _queueLength = (bufferMillis + (_ticksPerFrame - 1)) / _ticksPerFrame; //Round up

        _disposeTokenSource = new CancellationTokenSource();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token, cancellationToken);
        _cancellationToken = _cancellationTokenSource.Token;
        _queuedFrames = new ConcurrentQueue<Frame>();
        _bufferPool = new ConcurrentQueue<byte[]>();
        for (int i = 0; i < _queueLength; i++)
            _bufferPool.Enqueue(new byte[maxFrameSize]);
        _queueLock = new SemaphoreSlim(_queueLength, _queueLength);
        // _silenceFrames = MaxSilenceFrames;

        _ = Run();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposeTokenSource?.Cancel();
            _disposeTokenSource?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _queueLock?.Dispose();
            _next.Dispose();
        }
        base.Dispose(disposing);
    }

    private Task Run()
    {
        return Task.Run(async () =>
        {
            try
            {
                while (!_isPreloaded && !_cancellationToken.IsCancellationRequested)
                    await Task.Delay(1).ConfigureAwait(false);

                long nextTick = Environment.TickCount;
                while (!_cancellationToken.IsCancellationRequested)
                {
                    long tick = Environment.TickCount;
                    long dist = nextTick - tick;
                    if (dist <= 0)
                    {
                        if (_queuedFrames.TryDequeue(out Frame frame))
                        {
                            _next.WriteHeader(_client.LastRtpSequence, _client.LastRtpTimestamp, false);
                            await _next.WriteAsync(frame.Buffer, 0, frame.Bytes).ConfigureAwait(false);
                            _bufferPool.Enqueue(frame.Buffer);
                            _queueLock.Release();
                            nextTick += _ticksPerFrame;
                            _client.LastRtpSequence++;
                            _client.LastRtpTimestamp += OpusConverter.FrameSamplesPerChannel;
                            _client.SentPackets += 1;
                            _client.SentOctets += (uint)frame.Bytes;
                            _client.LastRtpActiveTick = Environment.TickCount;
                            // _silenceFrames = 0;
                            KookDebugger.DebugAudio($"Sent {frame.Bytes} bytes ({_queuedFrames.Count} frames buffered)");
                        }
                        else
                        {
                            // while (nextTick - tick <= 0)
                            // {
                            //     if (_silenceFrames++ < MaxSilenceFrames)
                            //     {
                            //         _next.WriteHeader(seq, _client._rtpTimestamp, false);
                            //         await _next.WriteAsync(_silenceFrame, 0, _silenceFrame.Length, CancellationToken.None)
                            //             .ConfigureAwait(false);
                            //     }
                            //
                            //     nextTick += _ticksPerFrame;
                            //     seq++;
                            //     _client._rtpTimestamp += OpusConverter.FrameSamplesPerChannel;
                            // }
                            KookDebugger.DebugAudio("[Audio] Buffer under run");
                        }
                    }
                    else
                        await Task.Delay((int)dist /*, _cancellationToken*/).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        });
    }

    /// <inheritdoc />
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        // Ignore, we use our own timing
    }

    /// <inheritdoc />
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        CancellationTokenSource? writeCancellationToken = null;
        if (cancellationToken.CanBeCanceled)
        {
            writeCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationToken);
            cancellationToken = writeCancellationToken.Token;
        }
        else
            cancellationToken = _cancellationToken;

        await _queueLock.WaitAsync(-1, cancellationToken).ConfigureAwait(false);
        if (!_bufferPool.TryDequeue(out byte[]? dstBuffer))
        {
            KookDebugger.DebugAudio("[Audio] Buffer overflow"); //Should never happen because of the queueLock
            writeCancellationToken?.Dispose();
            return;
        }
        Buffer.BlockCopy(buffer, offset, dstBuffer, 0, count);
        _queuedFrames.Enqueue(new Frame(dstBuffer, count));
        if (!_isPreloaded && _queuedFrames.Count == _queueLength)
        {
            KookDebugger.DebugAudio("[Audio] Preloaded");
            _isPreloaded = true;
        }
        writeCancellationToken?.Dispose();
    }

    /// <inheritdoc />
    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_queuedFrames.IsEmpty)
                return;
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken)
    {
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
        } while (_queuedFrames.TryDequeue(out _));
        return Task.Delay(TimeSpan.Zero, CancellationToken.None);
    }
}
