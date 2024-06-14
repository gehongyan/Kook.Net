namespace Kook.Net.Queue.SynchronousImmediate;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance of <see cref="SynchronousImmediateMessageQueue"/>.
/// </summary>
public static class SynchronousImmediateMessageQueueProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="MessageQueueProvider"/> instance.
    /// </summary>
    public static readonly MessageQueueProvider Instance = eventHandler =>
    {
        try
        {
            return new SynchronousImmediateMessageQueue(eventHandler);
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException(
                "The default SynchronousImmediateMessageQueueProvider is not supported on this platform.", ex);
        }
    };
}
