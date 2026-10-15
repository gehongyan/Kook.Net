using System.Collections.Concurrent;
using System.Diagnostics;
using Kook.Logging;

namespace Kook.Audio.WebRtc.Streams;

/// <summary>
///     表示一个定时缓冲的 WebRTC 音频写入流，按固定的帧间隔将缓冲的 Opus 包写入到下一个音频流中。
/// </summary>
/// <remarks>
///     与 <see cref="Kook.Audio.Streams.BufferedWriteStream"/> 的职责相同，区别是它不依赖 UDP 版本的
///     <c>AudioClient</c> 维护的 RTP 计数器，RTP 头部由 WebRTC 传输生成。
/// </remarks>
internal sealed class WebRtcBufferedWriteStream : AudioOutStream
{
    private struct Frame(byte[] buffer, int bytes)
    {
        public readonly byte[] Buffer = buffer;
        public readonly int Bytes = bytes;
    }

    private readonly AudioStream _next;
    private readonly CancellationTokenSource _disposeTokenSource, _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentQueue<Frame> _queuedFrames;
    private readonly ConcurrentQueue<byte[]> _bufferPool;
    private readonly SemaphoreSlim _queueLock;
    private readonly Logger _logger;
    private readonly int _ticksPerFrame, _queueLength;
    private bool _isPreloaded;

    internal WebRtcBufferedWriteStream(AudioStream next, int bufferMillis,
        CancellationToken cancellationToken, Logger logger, int maxFrameSize = 1500)
    {
        _next = next;
        _ticksPerFrame = WebRtcAudioFormat.FrameMillis;
        _logger = logger;
        _queueLength = (bufferMillis + (_ticksPerFrame - 1)) / _ticksPerFrame; // Round up

        _disposeTokenSource = new CancellationTokenSource();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeTokenSource.Token, cancellationToken);
        _cancellationToken = _cancellationTokenSource.Token;
        _queuedFrames = new ConcurrentQueue<Frame>();
        _bufferPool = new ConcurrentQueue<byte[]>();
        for (int i = 0; i < _queueLength; i++)
            _bufferPool.Enqueue(new byte[maxFrameSize]);
        _queueLock = new SemaphoreSlim(_queueLength, _queueLength);

        _ = Run();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposeTokenSource.Cancel();
            _disposeTokenSource.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _queueLock.Dispose();
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

                // Environment.TickCount 在 Windows 上的分辨率约为 15.6 毫秒，实测会让每一帧的发送时刻产生最多约 28 毫秒的误差，
                // 因此使用 Stopwatch，并以绝对时刻计算每一帧的发送时间，避免误差累积。
                Stopwatch clock = Stopwatch.StartNew();
                double nextTick = clock.Elapsed.TotalMilliseconds;
                while (!_cancellationToken.IsCancellationRequested)
                {
                    double dist = nextTick - clock.Elapsed.TotalMilliseconds;
                    if (dist <= 0)
                    {
                        if (_queuedFrames.TryDequeue(out Frame frame))
                        {
                            await _next.WriteAsync(frame.Buffer, 0, frame.Bytes).ConfigureAwait(false);
                            _bufferPool.Enqueue(frame.Buffer);
                            _queueLock.Release();
                            nextTick += _ticksPerFrame;
                            KookDebugger.DebugAudio($"[Audio] Sent a packet [Bytes] {frame.Bytes} [Buffered Frames] {_queuedFrames.Count}");
                        }
                        else
                        {
                            // 缓冲区耗尽后，下一次写入的数据应当立即发出，因此重置发送时刻，避免之后连发追赶
                            KookDebugger.DebugAudio("[Audio] Buffer under run");
                            _isPreloaded = false;
                            while (!_isPreloaded && !_cancellationToken.IsCancellationRequested)
                                await Task.Delay(1).ConfigureAwait(false);
                            nextTick = clock.Elapsed.TotalMilliseconds;
                        }
                    }
                    else
                        await Task.Delay(TimeSpan.FromMilliseconds(dist)).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("The WebRTC buffered write stream faulted", ex).ConfigureAwait(false);
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
            KookDebugger.DebugAudio("[Audio] Buffer overflow"); // Should never happen because of the queueLock
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
        // 缓冲区未被填满时发送循环不会启动，此时直接开始发送已有的数据
        if (!_isPreloaded && !_queuedFrames.IsEmpty)
            _isPreloaded = true;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_queuedFrames.IsEmpty)
                return;
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken)
    {
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
        } while (_queuedFrames.TryDequeue(out _));
        return Task.CompletedTask;
    }
}
