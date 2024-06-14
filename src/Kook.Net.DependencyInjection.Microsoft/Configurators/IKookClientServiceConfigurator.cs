using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     Represents a configurator for a Kook client.
/// </summary>
public interface IKookClientServiceConfigurator : IKookClientConfiguratorCompleter
{
    /// <summary>
    ///     Configures the Kook service to use the REST client.
    /// </summary>
    /// <param name="configure"> The configuration action. </param>
    /// <returns> The configurator. </returns>
    IKookClientConfigurator<KookRestClient, KookRestConfig> UseRestClient(Action<KookRestConfig> configure);

    /// <summary>
    ///     Configures the Kook service to use the socket client.
    /// </summary>
    /// <param name="configure"> The configuration action. </param>
    /// <returns> The configurator. </returns>
    IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseSocketClient(Action<KookSocketConfig> configure);

    /// <summary>
    ///     Configures the Kook service to use the webhook client.
    /// </summary>
    /// <param name="clientFactory"> The client factory. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <typeparam name="TClient"> The type of the client. </typeparam>
    /// <typeparam name="TConfig"> The type of the configuration. </typeparam>
    /// <returns> The configurator. </returns>
    IKookClientConfigurator<TClient, TConfig> UseWebhookClient<TClient, TConfig>(
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig;
}
