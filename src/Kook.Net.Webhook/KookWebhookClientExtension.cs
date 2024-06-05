using Kook.Net.Webhooks;
using Kook.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     Adds a KOOK webhook endpoint to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="builder"> The <see cref="IApplicationBuilder" /> to add the KOOK webhook endpoint to. </param>
    /// <param name="route"> The route to add the KOOK webhook endpoint to. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    public static WebApplication UseKookEndpoint(this WebApplication builder, string? route = null)
    {
        KookWebhookClient kookWebhookClient = builder.Services.GetRequiredService<KookWebhookClient>();
        if (kookWebhookClient.ApiClient.WebhookClient is not IAspNetWebhookClient aspNetWebhookClient)
            throw new InvalidOperationException("The Kook webhook client is not an AspNetWebhookClient.");
        string kookRoute = route
            ?? builder.Services.GetRequiredService<IOptions<KookWebhookConfig>>().Value.RouteEndpoint;
        builder.MapPost(kookRoute, async httpContext =>
            await aspNetWebhookClient.HandleRequestAsync(httpContext));
        return builder;
    }
}
