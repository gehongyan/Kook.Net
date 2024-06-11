using System.Text.Json;
using MassTransit;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance of <see cref="MassTransitMessageQueue"/>.
/// </summary>
public class DefaultMassTransitMessageQueueProvider
{
    /// <summary>
    ///     Creates a delegate that provides a new <see cref="IMessageQueue"/> instance of <see cref="MassTransitMessageQueue"/>.
    /// </summary>
    /// <param name="bus"> The MassTransit bus. </param>
    /// <returns> A new <see cref="IMessageQueue"/> instance of <see cref="MassTransitMessageQueue"/>. </returns>
    /// <exception cref="PlatformNotSupportedException"> The default <see cref="DefaultMassTransitMessageQueueProvider"/> is not supported on this platform. </exception>
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
