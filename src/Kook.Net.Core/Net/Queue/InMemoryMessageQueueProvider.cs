namespace Kook.Net.Queue;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance of <see cref="InMemoryMessageQueue"/>.
/// </summary>
public static class InMemoryMessageQueueProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="MessageQueueProvider"/> instance.
    /// </summary>
    public static readonly MessageQueueProvider Instance = () =>
    {
        try
        {
            return new InMemoryMessageQueue();
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException(
                "The default InMemoryMessageQueueProvider is not supported on this platform.", ex);
        }
    };
}

