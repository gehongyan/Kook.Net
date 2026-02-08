using System.Diagnostics.CodeAnalysis;
using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     提供用于与 Microsoft.Extensions.DependencyInjection 集成，注册与配置 Kook.Net 客户端作为服务的扩展方法。
/// </summary>
public static partial class KookClientDependencyInjectionExtensions
{
    #region REST

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Rest.KookRestClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookRestClient(this IServiceCollection services, string? serviceKey, Action<KookRestConfig> configure)
    {
        services.Configure(serviceKey, configure);
        services.AddKeyedKookRestClient(serviceKey);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Rest.KookRestClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="config"> 用于配置 KOOK REST 客户端的配置。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookRestClient(this IServiceCollection services, string? serviceKey, KookRestConfig config)
    {
        services.AddKeyedSingleton(serviceKey, config);
        services.AddKeyedKookRestClient(serviceKey);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Rest.KookRestClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookRestClient(this IServiceCollection services, string? serviceKey)
    {
        services.AddKeyedSingleton<KookRestClient>(serviceKey, (provider, key) => provider.GetKeyedService<IOptionsMonitor<KookRestConfig>>(key)?.Get(serviceKey) is { } config
            ? new KookRestClient(config)
            : new KookRestClient());
        services.AddKeyedSingleton<IKookClient, KookRestClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<KookRestClient>(key));
        services.AddKeyedSingleton<BaseKookClient, KookRestClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<KookRestClient>(key));
        return services;
    }

    #endregion

    #region Socket

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookSocketClient(this IServiceCollection services, string? serviceKey, Action<KookSocketConfig> configure)
    {
        services.Configure(serviceKey, configure);
        services.AddKeyedKookSocketClient(serviceKey);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="config"> 用于配置 KOOK 网关客户端的配置。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookSocketClient(this IServiceCollection services, string? serviceKey, KookSocketConfig config)
    {
        services.AddKeyedSingleton(serviceKey, config);
        services.AddKeyedKookSocketClient(serviceKey);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookSocketClient(this IServiceCollection services, string? serviceKey)
    {
        services.AddKeyedSingleton<KookSocketClient>(serviceKey, (provider, key) => provider.GetKeyedService<IOptionsMonitor<KookSocketConfig>>(key)?.Get(serviceKey) is { } config
            ? new KookSocketClient(config)
            : new KookSocketClient());
        services.AddKeyedSingleton<IKookClient, KookSocketClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<KookSocketClient>(key));
        services.AddKeyedSingleton<BaseKookClient, KookSocketClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<KookSocketClient>(key));
        services.AddKeyedSingleton<BaseSocketClient, KookSocketClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<KookSocketClient>(key));
        return services;
    }

    #endregion

    #region Webhook

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <param name="config"> 用于配置 KOOK Webhook 客户端的配置。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        string? serviceKey, Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {
        services.AddKeyedSingleton(serviceKey, config);
        services.AddKeyedKookWebhookClient<TClient>(serviceKey, (provider, _) => clientFactory(provider, config));
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookWebhookClient<TClient,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TConfig>(this IServiceCollection services,
        string? serviceKey, Func<IServiceProvider, TConfig, TClient> clientFactory)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {

        services.AddKeyedSingleton<TClient>(serviceKey, (provider, _) =>
            clientFactory(provider, provider.GetRequiredService<IOptionsMonitor<TConfig>>().Get(serviceKey)));
        services.AddKeyedKookWebhookClient<TClient>(serviceKey);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="serviceKey"> 服务的键。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKeyedKookWebhookClient<TClient>(this IServiceCollection services,
        string? serviceKey, Func<IServiceProvider, string?, TClient> clientFactory)
        where TClient: KookWebhookClient
    {
        services.AddKeyedSingleton<TClient>(serviceKey, (provider, _) => clientFactory(provider, serviceKey));
        services.AddKeyedKookWebhookClient<TClient>(serviceKey);
        return services;
    }

    internal static void AddKeyedKookWebhookClient<TClient>(this IServiceCollection services, string? serviceKey)
        where TClient: KookWebhookClient
    {
        services.AddKeyedSingleton<IKookClient, TClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<TClient>(key));
        services.AddKeyedSingleton<BaseKookClient, TClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<TClient>(key));
        services.AddKeyedSingleton<BaseSocketClient, TClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<TClient>(key));
        services.AddKeyedSingleton<KookSocketClient, TClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<TClient>(key));
        services.AddKeyedSingleton<KookWebhookClient, TClient>(serviceKey, (provider, key) => provider.GetRequiredKeyedService<TClient>(key));
    }

    #endregion
}
