using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     表示一个 KOOK 客户端配置器。
/// </summary>
public class KookClientServiceConfigurator : IKookClientServiceConfigurator
{
    private readonly IServiceCollection _services;
    private IKookClientConfiguratorCompleter? _kookClientConfigurator;

    internal KookClientServiceConfigurator(IServiceCollection services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public IKookClientConfigurator<KookRestClient, KookRestConfig> UseRestClient(Action<KookRestConfig> configure)
    {
        KookRestClientConfigurator configurator = new(_services, configure);
        _kookClientConfigurator = configurator;
        return configurator;
    }

    /// <inheritdoc />
    public IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseSocketClient(Action<KookSocketConfig> configure)
    {
        KookSocketClientConfigurator configurator = new(_services, configure);
        _kookClientConfigurator = configurator;
        return configurator;
    }

    /// <inheritdoc />
    public IKookClientConfigurator<TClient, TConfig> UseWebhookClient<TClient, TConfig>(
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        KookWebhookClientConfigurator<TClient, TConfig> configurator = new(_services, clientFactory, configure);
        _kookClientConfigurator = configurator;
        return configurator;
    }

    /// <inheritdoc />
    public void Complete()
    {
        _kookClientConfigurator?.Complete();
    }
}
