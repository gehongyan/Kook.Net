using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     Represents a base class for message queue.
/// </summary>
public abstract class BaseMessageQueue : IMessageQueue
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseMessageQueue"/> class.
    /// </summary>
    /// <param name="eventHandler"> The event handler for the message queue. </param>
    protected BaseMessageQueue(Func<JsonElement, Task> eventHandler)
    {
        EventHandler = eventHandler;
    }

    /// <summary>
    ///     Gets the event handler for the message queue.
    /// </summary>
    protected Func<JsonElement, Task> EventHandler { get; }

    /// <summary>
    ///     Starts the message queue processing.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token to cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops the message queue processing.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token to cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default);
}
