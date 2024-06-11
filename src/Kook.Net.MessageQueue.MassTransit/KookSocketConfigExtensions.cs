using Kook.WebSocket;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     Provides extension methods for <see cref="KookSocketConfig"/>
/// </summary>
public static class KookSocketConfigExtensions
{
    /// <summary>
    ///     Uses the MassTransit message queue.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceProvider"/> instance. </param>
    /// <param name="config"> The <see cref="KookSocketConfig"/> instance. </param>
    /// <returns> The <see cref="KookSocketConfig"/> instance. </returns>
    public static T UseMassTransitMessageQueue<T>(this IServiceProvider services, T config)
        where T : KookSocketConfig
    {
        IBus bus = services.GetRequiredService<IBus>();
        config.MessageQueueProvider = DefaultMassTransitMessageQueueProvider.Create(bus);
        return config;
    }
}
