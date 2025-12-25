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
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services, Action<KookRestConfig> configure)
    {
        services.Configure(configure);
        services.AddKookRestClient();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Rest.KookRestClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK REST 客户端的配置。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services, KookRestConfig config)
    {
        services.AddSingleton(config);
        services.AddKookRestClient();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Rest.KookRestClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端的服务集合。 </param>
    /// <returns> 添加了 KOOK REST 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services)
    {
        services.AddSingleton<KookRestClient>(provider => provider.GetService<IOptions<KookRestConfig>>()?.Value is { } config
            ? new KookRestClient(config)
            : new KookRestClient());
        services.AddSingleton<IKookClient, KookRestClient>(provider => provider.GetRequiredService<KookRestClient>());
        services.AddSingleton<BaseKookClient, KookRestClient>(provider => provider.GetRequiredService<KookRestClient>());
        return services;
    }

    #endregion

    #region Socket

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services, Action<KookSocketConfig> configure)
    {
        services.Configure(configure);
        services.AddKookSocketClient();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK 网关客户端的配置。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services, KookSocketConfig config)
    {
        services.AddSingleton(config);
        services.AddKookSocketClient();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.WebSocket.KookSocketClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端的服务集合。 </param>
    /// <returns> 添加了 KOOK 网关客户端的服务集合。 </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services)
    {
        services.AddSingleton<KookSocketClient>(provider => provider.GetService<IOptions<KookSocketConfig>>()?.Value is { } config
            ? new KookSocketClient(config)
            : new KookSocketClient());
        services.AddSingleton<IKookClient, KookSocketClient>(provider => provider.GetRequiredService<KookSocketClient>());
        services.AddSingleton<BaseKookClient, KookSocketClient>(provider => provider.GetRequiredService<KookSocketClient>());
        services.AddSingleton<BaseSocketClient, KookSocketClient>(provider => provider.GetRequiredService<KookSocketClient>());
        return services;
    }

    #endregion

    #region Webhook

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <param name="configure"> 用于配置 KOOK Webhook 客户端的配置委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookWebhookClient<TClient,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TConfig>(
        this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.Configure(configure);
        services.AddKookWebhookClient(clientFactory);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <param name="config"> 用于配置 KOOK Webhook 客户端的配置。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {
        services.AddSingleton(config);
        services.AddKookWebhookClient(clientFactory);
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookWebhookClient<TClient,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TConfig>(
        this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddSingleton<TClient>(provider =>
            clientFactory(provider, provider.GetRequiredService<IOptions<TConfig>>()));
        services.AddKookWebhookClient<TClient>();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> Webhook 客户端的配置类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {
        services.AddSingleton<TClient>(provider => clientFactory(provider, provider.GetRequiredService<TConfig>()));
        services.AddKookWebhookClient<TClient>();
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
    ///     添加 <see cref="Kook.Webhook.KookWebhookClient"/> 客户端。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK Webhook 客户端的服务集合。 </param>
    /// <param name="clientFactory"> 用于创建 KOOK Webhook 客户端的委托。 </param>
    /// <typeparam name="TClient"> Webhook 客户端的类型。 </typeparam>
    /// <returns> 添加了 KOOK Webhook 客户端的服务集合。 </returns>
    public static IServiceCollection AddKookWebhookClient<TClient>(this IServiceCollection services,
        Func<IServiceProvider, TClient> clientFactory)
        where TClient: KookWebhookClient
    {
        services.AddSingleton(clientFactory);
        services.AddKookWebhookClient<TClient>();
        return services;
    }

    internal static void AddKookWebhookClient<TClient>(this IServiceCollection services)
        where TClient: KookWebhookClient
    {
        services.AddSingleton<IKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookWebhookClient, TClient>(provider => provider.GetRequiredService<TClient>());
    }

    #endregion
}
