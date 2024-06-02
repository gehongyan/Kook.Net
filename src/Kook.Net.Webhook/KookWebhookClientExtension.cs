using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kook.Webhook;

/// <summary>
///     Provides extension methods for Kook webhook client.
/// </summary>
public static class KookWebhookClientExtension
{
    /// <summary>
    ///     Adds a KOOK webhook client to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection" /> to add the KOOK webhook client to. </param>
    /// <param name="configure"> The <see cref="KookSocketConfig" /> to configure the KOOK webhook client with. </param>
    /// <returns> The <see cref="IServiceCollection" /> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient(this IServiceCollection services, Action<KookWebhookConfig> configure)
    {
        services.Configure(configure);
        services.AddSingleton<KookWebhookClient>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookWebhookClient>());
#if NET5_0_OR_GREATER
        services.AddControllers();
#endif
        return services;
    }
}
