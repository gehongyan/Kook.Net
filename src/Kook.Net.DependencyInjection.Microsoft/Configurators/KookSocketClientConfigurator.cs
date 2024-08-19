using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.DependencyInjection.Microsoft;

internal class KookSocketClientConfigurator : KookClientConfigurator<KookSocketClient, KookSocketConfig>
{
    internal KookSocketClientConfigurator(IServiceCollection services, Action<KookSocketConfig> configure)
        : base(services, configure)
    {
        AppendService(service => service.AddKookSocketClient(configure));
    }
}
