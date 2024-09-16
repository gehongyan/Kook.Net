using Kook.Net.DependencyInjection.Microsoft;
using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Net.Hosting;

/// <summary>
///     提供用于与 Microsoft.Extensions.Hosting 集成，注册与配置 Kook.Net 客户端作为服务的扩展方法。
/// </summary>
public static class KookClientHostExtensions
{
    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Rest.KookRestClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端及服务的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK REST 客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK REST 客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookRestClient(this IServiceCollection services,
        KookRestConfig config, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookRestClient(config);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(provider.GetRequiredService<KookRestClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Rest.KookRestClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端及服务的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK REST 客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookRestClient(this IServiceCollection services,
        Action<KookRestConfig> configure, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookRestClient(configure);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(provider.GetRequiredService<KookRestClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Rest.KookRestClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端及服务的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK REST 客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookRestClient(this IServiceCollection services,
        Action<KookRestConfig> configure, Func<IServiceProvider, TokenType> tokenType,
        Func<IServiceProvider, string> token, Func<IServiceProvider, bool>? validateToken = null)
    {
        services.AddKookRestClient(configure);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(
            provider.GetRequiredService<KookRestClient>(), tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Rest.KookRestClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK REST 客户端及服务的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK REST 客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK REST 客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookRestClient(this IServiceCollection services,
        KookRestConfig config, Func<IServiceProvider, TokenType> tokenType,
        Func<IServiceProvider, string> token, Func<IServiceProvider, bool>? validateToken = null)
    {
        services.AddKookRestClient(config);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(
            provider.GetRequiredService<KookRestClient>(), tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.WebSocket.KookSocketClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookSocketClient(this IServiceCollection services,
        Action<KookSocketConfig> configure, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookSocketClient(configure);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(provider.GetRequiredService<KookSocketClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.WebSocket.KookSocketClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK 网关客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookSocketClient(this IServiceCollection services,
        KookSocketConfig config, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookSocketClient(config);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(provider.GetRequiredService<KookSocketClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.WebSocket.KookSocketClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookSocketClient(this IServiceCollection services,
        Action<KookSocketConfig> configure, Func<IServiceProvider, TokenType> tokenType,
        Func<IServiceProvider, string> token, Func<IServiceProvider, bool>? validateToken = null)
    {
        services.AddKookSocketClient(configure);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(
            provider.GetRequiredService<KookSocketClient>(), tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.WebSocket.KookSocketClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="config"> 用于配置 KOOK 网关客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <returns> 添加了 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookSocketClient(this IServiceCollection services,
        KookSocketConfig config, Func<IServiceProvider, TokenType> tokenType,
        Func<IServiceProvider, string> token, Func<IServiceProvider, bool>? validateToken = null)
    {
        services.AddKookSocketClient(config);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(
            provider.GetRequiredService<KookSocketClient>(), tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Webhook.KookWebhookClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="configure"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 添加了基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, configure);
        if (typeof(TClient).IsAssignableTo(typeof(IHostedService)))
            services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>());
        else
            services.AddHostedService(provider => new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Webhook.KookWebhookClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="config"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 添加了基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, config);
        if (typeof(TClient).IsAssignableTo(typeof(IHostedService)))
            services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>());
        else
            services.AddHostedService(provider => new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Webhook.KookWebhookClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="configure"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 添加了基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure,
        TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, configure);
        if (typeof(TClient).IsAssignableTo(typeof(IHostedService)))
            services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>());
        else
            services.AddHostedService(provider => new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType, token, validateToken));
        return services;
    }

    /// <summary>
    ///     向指定的 <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加
    ///     <see cref="Kook.Webhook.KookWebhookClient" /> 客户端，并添加包装为
    ///     <see cref="Microsoft.Extensions.Hosting.IHostedService" /> 的服务。
    /// </summary>
    /// <param name="services"> 要向其添加基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="config"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否在登录前验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 添加了基于 Webhook 的 KOOK 网关客户端及服务的服务集合。 </returns>
    public static IServiceCollection AddHostedKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config,
        TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, config);
        if (typeof(TClient).IsAssignableTo(typeof(IHostedService)))
            services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>());
        else
            services.AddHostedService(provider => new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType, token, validateToken));
        return services;
    }
}
