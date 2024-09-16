using Kook.Net.DependencyInjection.Microsoft;
using Kook.WebSocket;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     提供用于配置 <see cref="Kook.Net.DependencyInjection.Microsoft.IKookClientConfigurator{TClient,TConfig}"/>
///     使用 MassTransit 消息队列的扩展方法。
/// </summary>
public static class KookMassTransitMessageQueueExtensions
{
    /// <summary>
    ///     配置一个 MassTransit 消息队列消费者到 <see cref="global::MassTransit.IBusRegistrationConfigurator"/>。
    /// </summary>
    /// <param name="configurator"> MassTransit 配置器。 </param>
    /// <returns> 配置了 MassTransit 消息队列消费者的配置器。 </returns>
    public static IBusRegistrationConfigurator AddMessageQueueMassTransitConsumer(
        this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<KookMessageQueueMassTransitConsumer>();
        return configurator;
    }

    /// <summary>
    ///     配置一个 KOOK 客户端配置器使用 MassTransit 消息队列。
    /// </summary>
    /// <param name="configurator"> KOOK 客户端配置器。 </param>
    /// <param name="massTransitConfigure"> MassTransit 配置操作。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了 MassTransit 消息队列的配置器。 </returns>
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
    ///     配置一个 KOOK 客户端配置器使用 MassTransit 消息队列。
    /// </summary>
    /// <param name="configurator"> KOOK 客户端配置器。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了 MassTransit 消息队列的配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseMassTransitMessageQueue<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator)
        where TClient : BaseSocketClient
        where TConfig : KookSocketConfig
    {
        configurator.UseMessageQueue(provider =>
            DefaultMassTransitMessageQueueProvider.Create(provider.GetRequiredService<IBus>()));
        return configurator;
    }
}
