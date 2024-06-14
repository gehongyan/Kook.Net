using Kook.Webhook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     Represents a configurator for a Kook webhook client.
/// </summary>
/// <typeparam name="TClient"> The type of the client. </typeparam>
/// <typeparam name="TConfig"> The type of the configuration. </typeparam>
public class KookWebhookClientConfigurator<TClient, TConfig> : KookClientConfigurator<TClient, TConfig>
    where TClient : KookWebhookClient
    where TConfig : KookWebhookConfig
{
    internal KookWebhookClientConfigurator(IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookWebhookClient(clientFactory, configure));
    }

    internal KookWebhookClientConfigurator(IServiceCollection services, Action<TConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookWebhookClient((provider, _) => provider.GetRequiredService<TClient>(), configure));
    }
}
