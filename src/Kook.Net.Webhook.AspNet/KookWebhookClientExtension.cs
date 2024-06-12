using Kook.Net.Extensions.DependencyInjection;
using Kook.Net.Webhooks.AspNet;
using Kook.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook.AspNet;

/// <summary>
///     Provides extension methods for Kook webhook client.
/// </summary>
public static class KookWebhookClientExtension
{
    /// <summary>
    ///     Adds a KOOK webhook client to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection" /> to add the KOOK webhook client to. </param>
    /// <param name="config"> The <see cref="KookSocketConfig" /> to configure the KOOK webhook client with. </param>
    /// <returns> The <see cref="IServiceCollection" /> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookAspNetWebhookClient(this IServiceCollection services, KookAspNetWebhookConfig config)
    {
        services.AddSingleton(config);
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>());
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     Adds a KOOK webhook client to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection" /> to add the KOOK webhook client to. </param>
    /// <param name="configure"> The <see cref="KookSocketConfig" /> to configure the KOOK webhook client with. </param>
    /// <returns> The <see cref="IServiceCollection" /> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookAspNetWebhookClient(this IServiceCollection services, Action<KookAspNetWebhookConfig> configure)
    {
        services.AddKookWebhookClient((_, config) => new KookAspNetWebhookClient(config), configure);
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>());
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     Adds a KOOK webhook client to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection" /> to add the KOOK webhook client to. </param>
    /// <returns> The <see cref="IServiceCollection" /> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookAspNetWebhookClient(this IServiceCollection services)
    {
        services.AddKookWebhookClient<KookAspNetWebhookClient>(provider =>
        {
            if (provider.GetService<IOptions<KookAspNetWebhookConfig>>() is { } options)
                return new KookAspNetWebhookClient(options);
            if (provider.GetService<KookAspNetWebhookConfig>() is { } config)
                return new KookAspNetWebhookClient(config);
            throw new InvalidOperationException(
                "The KookAspNetWebhookConfig instance is not found in the service provider. Please make sure to add it.");
        });
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>());
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     Adds a KOOK webhook endpoint to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="builder"> The <see cref="IApplicationBuilder" /> to add the KOOK webhook endpoint to. </param>
    /// <param name="routePattern"> The route pattern to use for the KOOK webhook endpoint. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    public static WebApplication UseKookEndpoint(this WebApplication builder, string? routePattern = null)
    {
        KookAspNetWebhookClient? kookWebhookClient = builder.Services.GetService<KookAspNetWebhookClient>();
        if (kookWebhookClient is null)
            throw new InvalidOperationException("The KOOK webhook client is not found in the service provider. Please make sure to add it using AddKookAspNetWebhookClient.");
        if (kookWebhookClient.ApiClient.WebhookClient is not IAspNetWebhookClient aspNetWebhookClient)
            throw new InvalidOperationException("The Kook webhook client is not an AspNetWebhookClient.");
        string kookRoute = routePattern ?? builder.Services.GetRequiredService<IOptions<KookAspNetWebhookConfig>>().Value.RoutePattern;
        builder.MapPost(kookRoute, async httpContext => await aspNetWebhookClient.HandleRequestAsync(httpContext));
        return builder;
    }
}
