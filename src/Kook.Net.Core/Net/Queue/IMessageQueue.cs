using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     Represents a message queue used to send and receive messages from KOOK gateway.
/// </summary>
public interface IMessageQueue
{
    /// <summary>
    ///     Enqueues a message to the queue.
    /// </summary>
    /// <param name="payload"> The payload of the message. </param>
    /// <param name="sequence"> The sequence of the message. </param>
    /// <param name="cancellationToken"> </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default);
}
