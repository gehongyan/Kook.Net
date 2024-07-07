using System.Collections.Concurrent;
using System.Text.Json;

namespace Kook.Net.Queue.InMemory;

internal class InMemoryMessageQueue : BaseMessageQueue
{
    private readonly BlockingCollection<JsonElement> _queue;
    private CancellationTokenSource? _cancellationTokenSource;

    internal InMemoryMessageQueue(Func<JsonElement, Task> eventHandler)
        : base(eventHandler)
    {
        _queue = [];
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task.Factory.StartNew(async () =>
        {
            foreach (JsonElement gatewayEvent in _queue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                await EventHandler(gatewayEvent).ConfigureAwait(false);
        }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        _queue.CompleteAdding();
        return Task.CompletedTask;
    }

    public override Task EnqueueAsync(JsonElement payload, int sequence,
        CancellationToken cancellationToken = default)
    {
        _queue.Add(payload, cancellationToken);
        return Task.CompletedTask;
    }
}
