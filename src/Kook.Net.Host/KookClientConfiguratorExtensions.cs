using Kook.Net.Extensions.DependencyInjection;
using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Net.Host;

/// <summary>
///     Provides extension methods for <see cref="IKookClientConfigurator{TClient, TConfig}"/> to configure hosted clients.
/// </summary>
public static class KookClientConfiguratorExtensions
{
    /// <summary>
    ///     Configures a hosted client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
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
    ///     Configures a hosted client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
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
    ///     Configures a hosted REST client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<KookRestClient, KookRestConfig> UseHostedRestClient(
        this IKookClientServiceConfigurator configurator, Action<KookRestConfig> configure,
        TokenType tokenType, string token, bool validateToken = true) =>
        configurator
            .UseRestClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     Configures a hosted REST client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<KookRestClient, KookRestConfig> UseHostedRestClient(
        this IKookClientServiceConfigurator configurator, Action<KookRestConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        configurator
            .UseRestClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     Configures a hosted socket client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseHostedSocketClient(
        this IKookClientServiceConfigurator configurator, Action<KookSocketConfig> configure,
        TokenType tokenType, string token, bool validateToken = true) =>
        configurator
            .UseSocketClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     Configures a hosted socket client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseHostedSocketClient(
        this IKookClientServiceConfigurator configurator, Action<KookSocketConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null) =>
        configurator
            .UseSocketClient(configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     Configures a hosted webhook client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="clientFactory"> The client factory. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <typeparam name="TClient"> The type of the client. </typeparam>
    /// <typeparam name="TConfig"> The type of the configuration. </typeparam>
    /// <returns> The configurator. </returns>
    public static IKookClientConfigurator<TClient, TConfig> UseHostedWebhookClient<TClient, TConfig>(
        this IKookClientServiceConfigurator configurator, Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory,
        Action<TConfig> configure, TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig =>
        configurator
            .UseWebhookClient(clientFactory, configure)
            .UseHostedClient(tokenType, token, validateToken);

    /// <summary>
    ///     Configures a hosted webhook client with the specified token.
    /// </summary>
    /// <param name="configurator"> The configurator. </param>
    /// <param name="clientFactory"> The client factory. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <param name="tokenType"> The token type. </param>
    /// <param name="token"> The token. </param>
    /// <param name="validateToken"> The value indicating whether to validate the token. </param>
    /// <typeparam name="TClient"> The type of the client. </typeparam>
    /// <typeparam name="TConfig"> The type of the configuration. </typeparam>
    /// <returns> The configurator. </returns>
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
