using Kook.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.DependencyInjection.Microsoft;

internal class KookRestClientConfigurator : KookClientConfigurator<KookRestClient, KookRestConfig>
{
    internal KookRestClientConfigurator(IServiceCollection services, Action<KookRestConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookRestClient(configure));
    }
}
