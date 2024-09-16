using Kook.Net.Queue;
using Kook.WebSocket;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     为 <see cref="Kook.Net.DependencyInjection.Microsoft.IKookClientConfigurator{TClient,TConfig}"/> 提供配置 KOOK 客户端的扩展方法。
/// </summary>
public static class KookClientConfiguratorExtensions
{
    /// <summary>
    ///     配置 KOOK 客户端使用指定的消息队列提供程序。
    /// </summary>
    /// <param name="configurator"> 配置器。 </param>
    /// <param name="messageQueueProvider"> 消息队列提供程序。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了消息队列的配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseMessageQueue<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator,
        MessageQueueProvider messageQueueProvider)
        where TClient : BaseSocketClient
        where TConfig : KookSocketConfig
    {
        configurator.AppendConfigure((provider, config) => config.MessageQueueProvider = messageQueueProvider);
        return configurator;
    }

    /// <summary>
    ///     配置 KOOK 客户端使用指定的消息队列提供程序。
    /// </summary>
    /// <param name="configurator"> 配置器。 </param>
    /// <param name="messageQueueProvider"> 消息队列提供程序。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了消息队列的配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseMessageQueue<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator,
        Func<IServiceProvider, MessageQueueProvider> messageQueueProvider)
        where TClient : BaseSocketClient
        where TConfig : KookSocketConfig
    {
        configurator.AppendConfigure((provider, config) => config.MessageQueueProvider = messageQueueProvider(provider));
        return configurator;
    }
}
