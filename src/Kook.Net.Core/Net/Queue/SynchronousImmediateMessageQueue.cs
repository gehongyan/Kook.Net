using System.Text.Json;

namespace Kook.Net.Queue.SynchronousImmediate;

/// <summary>
///     Represents a synchronous immediate message queue.
/// </summary>
public class SynchronousImmediateMessageQueue : BaseMessageQueue
{
    /// <inheritdoc />
    public SynchronousImmediateMessageQueue(Func<JsonElement, Task> eventHandler)
        : base(eventHandler)
    {
    }

    /// <inheritdoc />
    public override async Task EnqueueAsync(JsonElement payload, int sequence,
        CancellationToken cancellationToken = default)
    {
        await EventHandler(payload).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
