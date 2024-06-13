using Kook.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Represents a configurator for a Kook REST client.
/// </summary>
public class KookRestClientConfigurator : KookClientConfigurator<KookRestClient, KookRestConfig>
{
    internal KookRestClientConfigurator(IServiceCollection services, Action<KookRestConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookRestClient(configure));
    }
}
