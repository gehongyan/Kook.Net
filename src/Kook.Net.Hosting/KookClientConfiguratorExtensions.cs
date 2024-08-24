using Kook.Net.DependencyInjection.Microsoft;
using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Net.Hosting;

/// <summary>
///     提供用于配置 <see cref="T:Kook.Net.DependencyInjection.Microsoft.IKookClientConfigurator`2"/>
///     添加客户端托管服务的扩展方法。
/// </summary>
public static class KookClientConfiguratorExtensions
{
    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseHostedClient<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator,
        TokenType tokenType, string token, bool validateToken = true)
        where TClient : BaseKookClient
        where TConfig : KookConfig
    {
        configurator.AppendService(typeof(TClient).IsAssignableTo(typeof(IHostedService))
            ? services => services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>())
            : services => services.AddHostedService(provider =>
                new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(), tokenType, token, validateToken)));
        return configurator;
    }

    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseHostedClient<TClient, TConfig>(
        this IKookClientConfigurator<TClient, TConfig> configurator,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null)
        where TClient : BaseKookClient
        where TConfig : KookConfig
    {
        configurator.AppendService(typeof(TClient).IsAssignableTo(typeof(IHostedService))
            ? services => services.AddSingleton(typeof(IHostedService), provider => provider.GetRequiredService<TClient>())
            : services => services.AddHostedService(provider =>
                new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                    tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true)));
        return configurator;
    }

    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加基于 REST 的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了 REST 客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<KookRestClient, KookRestConfig> UseHostedRestClient(
        this IKookClientServiceConfigurator configurator, Action<KookRestConfig> configure,
        TokenType tokenType, string token, bool validateToken = true) =>
        configurator
            .UseRestClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加基于 REST 的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="configure"> 用于配置 KOOK REST 客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了 REST 客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<KookRestClient, KookRestConfig> UseHostedRestClient(
        this IKookClientServiceConfigurator configurator, Action<KookRestConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        configurator
            .UseRestClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加基于网关的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了网关客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseHostedSocketClient(
        this IKookClientServiceConfigurator configurator, Action<KookSocketConfig> configure,
        TokenType tokenType, string token, bool validateToken = true) =>
        configurator
            .UseSocketClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     配置指定的 KOOK 客户端配置器添加基于网关的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="configure"> 用于配置 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <returns> 配置了网关客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseHostedSocketClient(
        this IKookClientServiceConfigurator configurator, Action<KookSocketConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        configurator
            .UseSocketClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     配置指定的基于 Webhook 的 KOOK 客户端配置器添加基于网关的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="configure"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了基于 Webhook 的网关客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseHostedWebhookClient<TClient, TConfig>(
        this IKookClientServiceConfigurator configurator, Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory,
        Action<TConfig> configure, TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig =>
        configurator
            .UseWebhookClient(clientFactory, configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     配置指定的基于 Webhook 的 KOOK 客户端配置器添加基于网关的客户端并添加客户端托管服务。
    /// </summary>
    /// <param name="configurator"> KOOK 服务配置器。 </param>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="configure"> 用于配置基于 Webhook 的 KOOK 网关客户端的配置委托。 </param>
    /// <param name="tokenType"> 令牌的类型。 </param>
    /// <param name="token"> 令牌。 </param>
    /// <param name="validateToken"> 是否验证令牌。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了基于 Webhook 的网关客户端及客户端托管服务的 KOOK 客户端配置器。 </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseHostedWebhookClient<TClient, TConfig>(
        this IKookClientServiceConfigurator configurator, Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory,
        Action<TConfig> configure, Func<IServiceProvider, TokenType> tokenType,
        Func<IServiceProvider, string> token, Func<IServiceProvider, bool>? validateToken = null)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig =>
        configurator
            .UseWebhookClient(clientFactory, configure)
            .UseHostedClient(tokenType, token, validateToken);
}
