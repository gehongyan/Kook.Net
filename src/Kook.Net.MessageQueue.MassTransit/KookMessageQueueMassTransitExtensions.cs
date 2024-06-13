using Kook.Net.Extensions.DependencyInjection;
using Kook.WebSocket;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     Provides extension methods for <see cref="IKookClientConfigurator{TClient, TConfig}"/> to configure MassTransit message queues.
/// </summary>
public static class KookMassTransitMessageQueueExtensions
{
    /// <summary>
    ///     Adds a MassTransit message queue consumer to the <see cref="IBusRegistrationConfigurator"/>.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <returns> The configurator. </returns>
    public static IBusRegistrationConfigurator AddMessageQueueMassTransitConsumer(
        this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<KookMessageQueueMassTransitConsumer>();
        return configurator;
    }

    /// <summary>
    ///     Configures a Kook client to use a MassTransit message queue.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="massTransitConfigure"> The MassTransit configuration action. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseMassTransitMessageQueue<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator,
        Action<IBusRegistrationConfigurator> massTransitConfigure)
        where TClient : BaseSocketClient
        where TConfig : KookSocketConfig
    {
        configurator.ServiceCollection.AddMassTransit(x =>
        {
            massTransitConfigure(x);
            x.AddMessageQueueMassTransitConsumer();
        });
        configurator.UseMassTransitMessageQueue();
        return configurator;
    }

    /// <summary>
    ///     Configures a Kook client to use a MassTransit message queue.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseMassTransitMessageQueue<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator)
        where TClient : BaseSocketClient
        where TConfig : KookSocketConfig
    {
        configurator.AppendConfigure((provider, config) =>
            config.MessageQueueProvider = DefaultMassTransitMessageQueueProvider.Create(provider.GetRequiredService<IBus>()));
        return configurator;
    }
}
