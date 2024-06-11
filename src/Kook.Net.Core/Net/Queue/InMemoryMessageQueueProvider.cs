using System.Text.Json;

namespace Kook.Net.Queue.InMemory;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance of <see cref="InMemoryMessageQueue"/>.
/// </summary>
public static class InMemoryMessageQueueProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="MessageQueueProvider"/> instance.
    /// </summary>
    public static readonly MessageQueueProvider Instance = eventHandler =>
    {
        try
        {
            return new InMemoryMessageQueue(eventHandler);
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException(
                "The default InMemoryMessageQueueProvider is not supported on this platform.", ex);
        }
    };
}

