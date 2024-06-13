using Kook.Webhook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Represents a configurator for a Kook webhook client.
/// </summary>
/// <typeparam name="TClient"> The type of the client. </typeparam>
/// <typeparam name="TConfig"> The type of the configuration. </typeparam>
public class KookWebhookClientConfigurator<TClient, TConfig> : KookClientConfigurator<KookWebhookClient, KookWebhookConfig>
    where TClient : KookWebhookClient
    where TConfig : KookWebhookConfig
{
    internal KookWebhookClientConfigurator(IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<KookWebhookConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookWebhookClient(clientFactory, configure));
    }
}
