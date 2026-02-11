using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace Kook.Net.Queue.InMemory;

/// <summary>
///     按网关序号 (sn) 顺序处理的内存消息队列，使用缓冲暂存乱序消息并在缺失序号时等待或超时处理。
/// </summary>
internal sealed class BufferedInMemoryMessageQueue : BaseMessageQueue
{
    private readonly InMemoryMessageQueueOptions _options;
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif
    private readonly SortedDictionary<int, JsonElement> _buffer = new();
    private readonly Channel<QueueItem> _outputChannel;
    private readonly int _maxSeq;

    private int _nextExpectedSn;
    private bool _initialized;
    private CancellationTokenSource? _cts;
    private Timer? _waitTimeoutTimer;
    private DateTimeOffset? _waitingSince;

    internal BufferedInMemoryMessageQueue(Func<int, JsonElement, Task> eventHandler, InMemoryMessageQueueOptions options)
        : base(eventHandler)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _maxSeq = _options.MaxSequenceNumber;
        _outputChannel = Channel.CreateUnbounded<QueueItem>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    /// <inheritdoc />
    public override bool HandleSequence => true;

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cts?.Dispose();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Task.Run(RunOutputReaderAsync, _cts.Token);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        _outputChannel.Writer.Complete();
        lock (_lock)
        {
            _waitTimeoutTimer?.Dispose();
            _waitTimeoutTimer = null;
            _waitingSince = null;
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default)
    {
        List<QueueItem>? toProcess = null;
        lock (_lock)
        {
            if (!_initialized)
            {
                _nextExpectedSn = sequence;
                _initialized = true;
            }

            int next = _nextExpectedSn;
            int mod = _maxSeq + 1;

            // 已处理过的序号直接丢弃
            if (IsAlreadyProcessed(sequence, next, mod))
                return Task.CompletedTask;

            // 期望的下一个序号，直接处理并尝试清空缓冲区
            if (sequence == next)
            {
                toProcess = [new QueueItem(sequence, payload)];
                _nextExpectedSn = NextSn(next, mod);
                StopWaitTimeoutTimer();
                _waitingSince = null;
                DrainBufferLocked(toProcess);
            }
            // 序号大于期望值，放入缓冲区
            else if (_buffer.Count < _options.BufferCapacity)
                _buffer[sequence] = payload;
            // 缓冲区已满，按策略处理
            else
            {
                switch (_options.BufferOverflowStrategy)
                {
                    // 丢弃新到的消息
                    case BufferOverflowStrategy.DropIncoming:
                        return Task.CompletedTask;
                    case BufferOverflowStrategy.ShiftOne:
                        int minSn = _buffer.Keys.MinBy(x => (x - _nextExpectedSn + mod) % mod);
                        JsonElement minPayload = _buffer[minSn];
                        _buffer.Remove(minSn);
                        toProcess = [new QueueItem(minSn, minPayload)];
                        _buffer[sequence] = payload;
                        break;
                    // 抛出异常
                    case BufferOverflowStrategy.ThrowException:
                        throw new InvalidOperationException(
                            $"Buffer overflow: capacity {_options.BufferCapacity} reached while enqueueing sequence {sequence}.");
                    // 请求重连
                    case BufferOverflowStrategy.ReconnectGateway:
                        OnReconnectRequested(new ReconnectRequestedEventArgs(
                            ReconnectRequestedReason.BufferOverflow,
                            new InvalidOperationException(
                                $"Buffer overflow: capacity {_options.BufferCapacity} reached while enqueueing sequence {sequence}.")));
                        return Task.CompletedTask;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_options.BufferOverflowStrategy), "Unknown buffer overflow strategy.");
                }
            }

            MaybeStartWaitTimeoutTimer();
        }

        return WriteToOutputChannelAsync(toProcess, cancellationToken);
    }

    private static bool IsAlreadyProcessed(int sn, int nextExpectedSn, int mod)
    {
        int diff = (sn - nextExpectedSn + mod) % mod;
        return diff >= mod / 2;
    }

    private static int NextSn(int sn, int mod) => (sn + 1) % mod;

    private void DrainBufferLocked(List<QueueItem> toProcess)
    {
        int mod = _maxSeq + 1;
        while (_buffer.TryGetValue(_nextExpectedSn, out JsonElement value))
        {
            int currentSn = _nextExpectedSn;
            _buffer.Remove(_nextExpectedSn);
            toProcess.Add(new QueueItem(currentSn, value));
            _nextExpectedSn = NextSn(_nextExpectedSn, mod);
        }
    }

    private void MaybeStartWaitTimeoutTimer()
    {
        if (_options.WaitForMissingTimeout is not { } timeout
            || timeout == Timeout.InfiniteTimeSpan
            || timeout <= TimeSpan.Zero)
            return;
        if (_buffer.Count == 0)
            return;
        if (_waitTimeoutTimer is not null)
            return;

        _waitingSince ??= DateTimeOffset.UtcNow;
        _waitTimeoutTimer = new Timer(
            OnWaitTimeout,
            null,
            timeout,
            Timeout.InfiniteTimeSpan);
    }

    private void StopWaitTimeoutTimer()
    {
        _waitTimeoutTimer?.Dispose();
        _waitTimeoutTimer = null;
    }

    private void OnWaitTimeout(object? state)
    {
        List<QueueItem>? toProcess = null;
        lock (_lock)
        {
            StopWaitTimeoutTimer();
            if (_buffer.Count == 0)
            {
                _waitingSince = null;
                return;
            }

            switch (_options.BufferWaitTimeoutStrategy)
            {
                case BufferWaitTimeoutStrategy.SkipMissing:
                    int mod = _maxSeq + 1;
                    int minSn = _buffer.Keys.MinBy(x => (x - _nextExpectedSn + mod) % mod);
                    _nextExpectedSn = minSn;
                    toProcess = [];
                    DrainBufferLocked(toProcess);
                    _waitingSince = null;
                    MaybeStartWaitTimeoutTimer();
                    break;
                case BufferWaitTimeoutStrategy.ReconnectGateway:
                    _waitingSince = null;
                    break;
            }
        }

        switch (_options.BufferWaitTimeoutStrategy)
        {
            case BufferWaitTimeoutStrategy.SkipMissing:
                if (toProcess is { Count: > 0 })
                    _ = WriteToOutputChannelAsync(toProcess, CancellationToken.None);
                break;
            case BufferWaitTimeoutStrategy.ReconnectGateway:
                OnReconnectRequested(new ReconnectRequestedEventArgs(
                    ReconnectRequestedReason.WaitTimeout,
                    new InvalidOperationException("Wait for missing sequence timeout. Requesting gateway reconnect.")));
                break;
        }
    }

    private async Task WriteToOutputChannelAsync(List<QueueItem>? toProcess, CancellationToken cancellationToken)
    {
        if (toProcess is null)
            return;
        foreach (QueueItem item in toProcess)
            await _outputChannel.Writer.WriteAsync(item, cancellationToken).ConfigureAwait(false);
    }

    private async Task RunOutputReaderAsync()
    {
        try
        {
            await foreach (QueueItem item in _outputChannel.Reader.ReadAllAsync(_cts!.Token).ConfigureAwait(false))
            {
                try
                {
                    await EventHandler(item.Sequence, item.Payload).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing message: {ex}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected when stopping
        }
    }
}
