namespace Kook.Net.Queue.InMemory;

/// <summary>
///     表示一个默认的使用内存队列的 <see cref="MessageQueueProvider"/>，用于创建 <see cref="InMemoryMessageQueue"/> 实例。
/// </summary>
public static class InMemoryMessageQueueProvider
{
    /// <summary>
    ///     创建一个新的用于创建默认的使用内存队列的 <see cref="InMemoryMessageQueue"/> 实例的委托。
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
