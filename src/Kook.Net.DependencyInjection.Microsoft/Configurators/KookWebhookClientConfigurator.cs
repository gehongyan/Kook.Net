using System.Diagnostics.CodeAnalysis;
using Kook.Webhook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

internal class KookWebhookClientConfigurator<TClient,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TConfig> :
    KookClientConfigurator<TClient, TConfig>
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
