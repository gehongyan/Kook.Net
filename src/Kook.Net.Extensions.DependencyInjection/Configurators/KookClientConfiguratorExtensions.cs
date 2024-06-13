using Kook.Net.Queue;
using Kook.WebSocket;

namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for <see cref="IKookClientConfigurator{TClient, TConfig}"/> to configure Kook clients.
/// </summary>
public static class KookClientConfiguratorExtensions
{
    /// <summary>
    ///     Configures a Kook client to use a message queue provider.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="messageQueueProvider"> The message queue provider. </param>
    /// <typeparam name="TClient"> The type of the client. </typeparam>
    /// <typeparam name="TConfig"> The type of the configuration. </typeparam>
    /// <returns> The configurator. </returns>
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
    ///     Configures a Kook client to use a message queue provider.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="messageQueueProvider"> The message queue provider. </param>
    /// <typeparam name="TClient"> The type of the client. </typeparam>
    /// <typeparam name="TConfig"> The type of the configuration. </typeparam>
    /// <returns> The configurator. </returns>
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
