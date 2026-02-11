namespace Kook.Net.Queue.InMemory;

/// <summary>
///     表示一个默认的使用内存队列的 <see cref="MessageQueueProvider"/>，用于创建 <see cref="InMemoryMessageQueue"/> 或带缓冲的 <see cref="BufferedInMemoryMessageQueue"/> 实例。
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

    /// <summary>
    ///     使用指定的选项创建一个新的用于创建默认的使用内存队列的 <see cref="MessageQueueProvider"/> 实例。
    /// </summary>
    /// <param name="options"> 内存队列配置。 </param>
    /// <returns> 用于创建消息队列的委托。 </returns>
    public static MessageQueueProvider Create(InMemoryMessageQueueOptions? options = null)
    {
        if (options?.EnableBuffering is not true)
            return Instance;

        return eventHandler =>
        {
            try
            {
                return new BufferedInMemoryMessageQueue(eventHandler, options);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException(
                    "The BufferedInMemoryMessageQueueProvider is not supported on this platform.", ex);
            }
        };
    }
}
