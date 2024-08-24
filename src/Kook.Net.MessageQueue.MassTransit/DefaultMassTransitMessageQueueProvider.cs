using MassTransit;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     表示一个默认的使用 MassTransit 消息队列的 <see cref="MessageQueueProvider"/>，用于创建 <see cref="MassTransitMessageQueue"/> 实例。
/// </summary>
public class DefaultMassTransitMessageQueueProvider
{
    /// <summary>
    ///     创建一个新的用于创建默认的使用 MassTransit 消息队列的 <see cref="MassTransitMessageQueue"/> 实例的委托。
    /// </summary>
    /// <param name="bus"> 用于创建消息队列的 MassTransit 消息总线。 </param>
    public static MessageQueueProvider Create(IBus bus)
    {
        try
        {
            return eventHandler => new MassTransitMessageQueue(eventHandler, bus);
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException(
                "The default DefaultMassTransitMessageQueueProvider is not supported on this platform.", ex);
        }
    }
}
