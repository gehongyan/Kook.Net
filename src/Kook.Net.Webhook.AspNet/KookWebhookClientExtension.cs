using System.Diagnostics.CodeAnalysis;
using Kook.Net.DependencyInjection.Microsoft;
using Kook.Net.Webhooks.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook.AspNet;

/// <summary>
///     提供用于与 ASP.NET 集成，注册与配置 <see cref="Kook.Webhook.AspNet.KookAspNetWebhookClient"/> 的扩展方法。
/// </summary>
public static partial class KookWebhookClientExtension
{
    /// <summary>
    ///     向指定的 <see cref="IServiceCollection" /> 添加 <see cref="Kook.Webhook.AspNet.KookAspNetWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK ASP.NET Webhook 客户端的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK ASP.NET Webhook 客户端的配置。 </param>
    /// <returns> 添加了 KOOK ASP.NET Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookAspNetWebhookClient(this IServiceCollection services, KookAspNetWebhookConfig config)
    {
        services.AddSingleton(config);
        services.AddKookWebhookClient(provider => new KookAspNetWebhookClient(provider.GetRequiredService<KookAspNetWebhookConfig>()));
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>());
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="IServiceCollection" /> 添加 <see cref="Kook.Webhook.AspNet.KookAspNetWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK ASP.NET Webhook 客户端的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK ASP.NET Webhook 客户端的配置委托。 </param>
    /// <returns> 添加了 KOOK ASP.NET Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookAspNetWebhookClient(this IServiceCollection services, Action<KookAspNetWebhookConfig> configure)
    {
        services.AddKookWebhookClient((_, config) => new KookAspNetWebhookClient(config), configure);
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<KookAspNetWebhookClient>());
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="IServiceCollection" /> 添加 KOOK Webhook 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK ASP.NET Webhook 客户端的服务集合。 </param>
    /// <returns> 添加了 KOOK ASP.NET Webhook 客户端的服务集合。 </returns>
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
    ///     配置 KOOK 服务以使用 ASP.NET Webhook 客户端。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="configure"> 用于配置 KOOK ASP.NET Webhook 客户端的配置委托。 </param>
    /// <returns> 配置了  KOOK ASP.NET Webhook 客户端的配置器。 </returns>
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
    ///     配置 KOOK 服务以将 ASP.NET Webhook 客户端包装为 <see cref="Microsoft.Extensions.Hosting.IHostedService"/>。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了 ASP.NET Webhook 客户端的配置器。 </returns>
    [DoesNotReturn]
    [Obsolete("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.")]
    public static IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> UseHostedClient(
        this IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> configurator,
        TokenType tokenType, string token, bool validateToken = true) =>
        throw new InvalidOperationException("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.");

    /// <summary>
    ///     配置 KOOK 服务以将 ASP.NET Webhook 客户端包装为 <see cref="Microsoft.Extensions.Hosting.IHostedService"/>。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了 ASP.NET Webhook 客户端的配置器。 </returns>
    [DoesNotReturn]
    [Obsolete("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.")]
    public static IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> UseHostedClient(
        this IKookClientConfigurator<KookAspNetWebhookClient, KookAspNetWebhookConfig> configurator,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        throw new InvalidOperationException("The KookAspNetWebhookClient itself is a hosted service, configure the token in the KookAspNetWebhookConfig.");

    /// <summary>
    ///     向 <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder" /> 添加 KOOK Webhook 路由端点。
    /// </summary>
    /// <param name="builder"> 要向其添加 KOOK Webhook 端点的应用程序构建器。 </param>
    /// <param name="routePattern"> 用于注册 KOOK Webhook 的路由模式。 </param>
    /// <returns> 添加了 KOOK Webhook 端点的应用程序构建器。 </returns>
    public static T UseKookEndpoint<T>(this T builder, string? routePattern = null)
        where T: IHost, IEndpointRouteBuilder
    {
        KookWebhookClient? kookWebhookClient = builder.Services.GetService<KookWebhookClient>();
        if (kookWebhookClient is null)
            throw new InvalidOperationException("The KOOK webhook client is not found in the service provider. Please make sure to add it using AddKookAspNetWebhookClient.");
        if (kookWebhookClient.ApiClient.WebhookClient is not IAspNetWebhookClient aspNetWebhookClient)
            throw new InvalidOperationException("The Kook webhook client is not an AspNetWebhookClient.");
        string kookRoute = routePattern ?? builder.Services.GetRequiredService<IOptions<KookAspNetWebhookConfig>>().Value.RoutePattern;
        builder.MapPost(kookRoute, async httpContext => await aspNetWebhookClient.HandleRequestAsync(httpContext));
        return builder;
    }
}
