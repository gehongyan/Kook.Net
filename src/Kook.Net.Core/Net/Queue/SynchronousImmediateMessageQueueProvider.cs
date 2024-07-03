namespace Kook.Net.Queue.SynchronousImmediate;

/// <summary>
///     表示一个默认的使用同步处理机制的 <see cref="MessageQueueProvider"/>，用于创建 <see cref="SynchronousImmediateMessageQueue"/> 实例。
/// </summary>
public static class SynchronousImmediateMessageQueueProvider
{
    /// <summary>
    ///     创建一个新的用于创建默认的使用同步处理机制的 <see cref="SynchronousImmediateMessageQueue"/> 实例的委托。
    /// </summary>
    /// <returns> 一个用于创建默认的使用同步处理机制的 <see cref="SynchronousImmediateMessageQueue"/> 实例的委托。 </returns>
    /// <exception cref="PlatformNotSupportedException"> 当默认的 <see cref="SynchronousImmediateMessageQueueProvider"/> 在当前平台上不受支持时引发。 </exception>
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
