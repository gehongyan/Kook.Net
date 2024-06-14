using System.Diagnostics.CodeAnalysis;
using Kook.Net.DependencyInjection.Microsoft;
using Kook.Net.Webhooks.AspNet;
using Kook.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
    ///     Configures the KOOK service to use the ASP.NET webhook client.
    /// </summary>
    /// <param name="configurator"> The KOOK service configurator. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <returns> The KOOK service configurator. </returns>
    public static IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> UseAspNetWebhookClient
        (this IKookClientServiceConfigurator configurator, Action<KookAspNetWebhookConfig> configure)
    {
        IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> serviceConfigurator =
            configurator.UseWebhookClient<KookAspNetWebhookClient, KookAspNetWebhookConfig>(
                (_, config) => new KookAspNetWebhookClient(config), configure);
        serviceConfigurator.AppendService(service =>
            service.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>()));
        serviceConfigurator.AppendService(service => service.AddControllers());
        return serviceConfigurator;
    }

    /// <summary>
    ///     Configures the KOOK service to use the ASP.NET webhook client.
    /// </summary>
    /// <param name="configurator"> The KOOK service configurator. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The KOOK service configurator. </returns>
    [DoesNotReturn]
    [Obsolete("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.")]
    public static IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> UseHostedClient(
        this IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> configurator,
        TokenType tokenType, string token, bool validateToken = true) =>
        throw new InvalidOperationException("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.");

    /// <summary>
    ///     Configures the KOOK service to use the ASP.NET webhook client.
    /// </summary>
    /// <param name="configurator"> The KOOK service configurator. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The KOOK service configurator. </returns>
    [DoesNotReturn]
    [Obsolete("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.")]
    public static IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> UseHostedClient(
        this IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> configurator,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        throw new InvalidOperationException("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.");

    /// <summary>
    ///     Adds a KOOK webhook endpoint to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="builder"> The <see cref="IApplicationBuilder" /> to add the KOOK webhook endpoint to. </param>
    /// <param name="routePattern"> The route pattern to use for the KOOK webhook endpoint. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    public static T UseKookEndpoint<T>(this T builder, string? routePattern = null)
        where T: IHost, IEndpointRouteBuilder
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
