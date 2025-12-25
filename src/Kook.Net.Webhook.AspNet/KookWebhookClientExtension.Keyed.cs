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
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="config"> 用于配置 KOOK ASP.NET Webhook 客户端的配置。 </param>
    /// <returns> 添加了 KOOK ASP.NET Webhook 客户端的服务集合。 </returns>
    [RequiresUnreferencedCode("MVC does not currently support trimming or native AOT.")]
    public static IServiceCollection AddKeyedKookAspNetWebhookClient(this IServiceCollection services, string? serviceKey, KookAspNetWebhookConfig config)
    {
        services.AddKeyedSingleton(serviceKey, config);
        services.AddKeyedKookWebhookClient(serviceKey, (provider, key) => new KookAspNetWebhookClient(provider.GetRequiredKeyedService<KookAspNetWebhookConfig>(key)));
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredKeyedService<KookAspNetWebhookClient>(serviceKey));
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="IServiceCollection" /> 添加 KOOK Webhook 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK ASP.NET Webhook 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <returns> 添加了 KOOK ASP.NET Webhook 客户端的服务集合。 </returns>
    [RequiresUnreferencedCode("MVC does not currently support trimming or native AOT.")]
    public static IServiceCollection AddKeyedKookAspNetWebhookClient(this IServiceCollection services, string? serviceKey)
    {
        services.AddKeyedKookWebhookClient<KookAspNetWebhookClient>(serviceKey, (provider, key) =>
        {
            if (provider.GetService<IOptionsMonitor<KookAspNetWebhookConfig>>()?.Get(key) is { } options)
                return new KookAspNetWebhookClient(options);
            if (provider.GetKeyedService<KookAspNetWebhookConfig>(key) is { } config)
                return new KookAspNetWebhookClient(config);
            throw new InvalidOperationException(
                "The KookAspNetWebhookConfig instance is not found in the service provider. Please make sure to add it.");
        });
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredKeyedService<KookAspNetWebhookClient>(serviceKey));
        services.AddControllers();
        return services;
    }

    /// <summary>
    ///     向 <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder" /> 添加 KOOK Webhook 路由端点。
    /// </summary>
    /// <param name="builder"> 要向其添加 KOOK Webhook 端点的应用程序构建器。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="routePattern"> 用于注册 KOOK Webhook 的路由模式。 </param>
    /// <returns> 添加了 KOOK Webhook 端点的应用程序构建器。 </returns>
    public static T UseKeyedKookEndpoint<T>(this T builder, string? serviceKey, string? routePattern = null)
        where T: IHost, IEndpointRouteBuilder
    {
        KookWebhookClient? kookWebhookClient = builder.Services.GetKeyedService<KookWebhookClient>(serviceKey);
        if (kookWebhookClient is null)
            throw new InvalidOperationException($"The KOOK webhook client keyed with {serviceKey} is not found in the service provider. Please make sure to add it using AddKeyedKookAspNetWebhookClient.");
        if (kookWebhookClient.ApiClient.WebhookClient is not IAspNetWebhookClient aspNetWebhookClient)
            throw new InvalidOperationException($"The Kook webhook client keyed with {serviceKey} is not an AspNetWebhookClient.");
        string kookRoute = routePattern ?? builder.Services.GetRequiredKeyedService<KookAspNetWebhookConfig>(serviceKey).RoutePattern;
        builder.MapPost(kookRoute, async httpContext => await aspNetWebhookClient.HandleRequestAsync(httpContext));
        return builder;
    }
}
