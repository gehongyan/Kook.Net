using System.Text.Json;

namespace Kook.Net.Queue;

internal class InMemoryMessageQueue : IMessageQueue
{
#if NET6_0_OR_GREATER
    private readonly PriorityQueue<JsonElement, int> _queue;
#else
    private readonly Queue<JsonElement> _queue;
#endif
    private readonly SemaphoreSlim _semaphore;
    private CancellationTokenSource? _dequeueCancellationToken;

    internal InMemoryMessageQueue()
    {
        _semaphore = new SemaphoreSlim(1, 1);
#if NET6_0_OR_GREATER
        _queue = new PriorityQueue<JsonElement, int>();
#else
        _queue = new Queue<JsonElement>();
#endif
    }

    public async Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
#if NET6_0_OR_GREATER
            _queue.Enqueue(payload, sequence);
#else
            _queue.Enqueue(payload);
#endif
            _dequeueCancellationToken?.Cancel();
            _dequeueCancellationToken?.Dispose();
            _dequeueCancellationToken = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<JsonElement> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_queue.Count > 0)
                return _queue.Dequeue();
            _dequeueCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, _dequeueCancellationToken.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        return _queue.Dequeue();
    }
}
