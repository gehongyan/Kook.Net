using Kook.Net.Extensions.DependencyInjection;
using Kook.Rest;
using Microsoft.Extensions.DependencyInjection;

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
        configurator.AppendService(services => services.AddSingleton<KookClientHostedService<TClient>>(provider =>
            new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(), tokenType, token, validateToken)));
        configurator.AppendService(services => services.AddHostedService<KookClientHostedService<TClient>>(provider =>
            provider.GetRequiredService<KookClientHostedService<TClient>>()));
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
        configurator.AppendService(services => services.AddSingleton<KookClientHostedService<TClient>>(provider =>
            new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true)));
        configurator.AppendService(services => services.AddHostedService<KookClientHostedService<TClient>>(provider =>
            provider.GetRequiredService<KookClientHostedService<TClient>>()));
        return configurator;
    }
}
