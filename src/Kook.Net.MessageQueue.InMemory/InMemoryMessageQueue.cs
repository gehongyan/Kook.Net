using System.Text.Json;

namespace Kook.Net.Queue.InMemory;

internal class InMemoryMessageQueue : BaseMessageQueue
{
#if NET6_0_OR_GREATER
    private readonly PriorityQueue<JsonElement, int> _queue;
#else
    private readonly Queue<JsonElement> _queue;
#endif
    private readonly SemaphoreSlim _semaphore;
    private CancellationTokenSource? _inMemoryQueueCancellationTokenSource;
    private CancellationTokenSource? _dequeueWaitingCancellationTokenSource;

    internal InMemoryMessageQueue(Func<JsonElement, Task> eventHandler)
        : base(eventHandler)
    {
        _semaphore = new SemaphoreSlim(1, 1);
#if NET6_0_OR_GREATER
        _queue = new PriorityQueue<JsonElement, int>();
#else
        _queue = new Queue<JsonElement>();
#endif
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _inMemoryQueueCancellationTokenSource?.Dispose();
        _inMemoryQueueCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task.Run(async () =>
        {
            while (!_inMemoryQueueCancellationTokenSource.Token.IsCancellationRequested)
            {
                JsonElement gatewayEvent = await DequeueAsync(_inMemoryQueueCancellationTokenSource.Token)
                    .ConfigureAwait(false);
                await EventHandler(gatewayEvent).ConfigureAwait(false);
            }
        }, _inMemoryQueueCancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        _inMemoryQueueCancellationTokenSource?.Cancel();
        _inMemoryQueueCancellationTokenSource?.Dispose();
        _inMemoryQueueCancellationTokenSource = null;
        return Task.CompletedTask;
    }

    public override async Task EnqueueAsync(JsonElement payload, int sequence,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
#if NET6_0_OR_GREATER
            _queue.Enqueue(payload, sequence);
#else
            _queue.Enqueue(payload);
#endif
            _dequeueWaitingCancellationTokenSource?.Cancel();
            _dequeueWaitingCancellationTokenSource?.Dispose();
            _dequeueWaitingCancellationTokenSource = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<JsonElement> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_queue.Count > 0)
                return _queue.Dequeue();
            _dequeueWaitingCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, _dequeueWaitingCancellationTokenSource.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        return _queue.Dequeue();
    }
}
