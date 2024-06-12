using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection;

/// <summary>
///     Provides extension methods for Kook.Net to add clients using the <see cref="IServiceCollection"/>.
/// </summary>
public static class KookClientDependencyInjectionExtensions
{
    /// <summary>
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services,
        KookRestConfig config, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookRestClient(config);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(provider.GetRequiredService<KookRestClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services,
        Action<KookRestConfig> configure, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookRestClient(configure);
        services.AddSingleton<KookClientHostedService<KookRestClient>>(provider => new KookClientHostedService<KookRestClient>(provider.GetRequiredService<KookRestClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookRestClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookRestClient>>());
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services,
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
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services,
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
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services, Action<KookRestConfig> configure)
    {
        services.Configure(configure);
        services.AddKookRestClient();
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookRestConfig"/>. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services, KookRestConfig config)
    {
        services.AddSingleton(config);
        services.AddKookRestClient();
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookRestClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookRestClient(this IServiceCollection services)
    {
        services.AddSingleton<KookRestClient>(provider => provider.GetService<IOptions<KookRestConfig>>()?.Value is { } config
            ? new KookRestClient(config)
            : new KookRestClient());
        services.AddSingleton<IKookClient, KookRestClient>(provider => provider.GetRequiredService<KookRestClient>());
        services.AddSingleton<BaseKookClient, KookRestClient>(provider => provider.GetRequiredService<KookRestClient>());
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services,
        Action<KookSocketConfig> configure, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookSocketClient(configure);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(provider.GetRequiredService<KookSocketClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services,
        KookSocketConfig config, TokenType tokenType, string token, bool validateToken = true)
    {
        services.AddKookSocketClient(config);
        services.AddSingleton<KookClientHostedService<KookSocketClient>>(provider => new KookClientHostedService<KookSocketClient>(provider.GetRequiredService<KookSocketClient>(), tokenType, token, validateToken));
        services.AddHostedService<KookClientHostedService<KookSocketClient>>(provider => provider.GetRequiredService<KookClientHostedService<KookSocketClient>>());
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services,
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
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services,
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
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="configure"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services, Action<KookSocketConfig> configure)
    {
        services.Configure(configure);
        services.AddKookSocketClient();
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="config"> The action to configure the <see cref="KookSocketConfig"/>. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookSocketClient(this IServiceCollection services, KookSocketConfig config)
    {
        services.AddSingleton(config);
        services.AddKookSocketClient();
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookSocketClient"/> to the specified <see cref="IServiceCollection"/> with
    ///     the specified configuration action and login information.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
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

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="configure"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, configure);
        services.AddHostedService(provider => provider.GetRequiredService<TClient>() as IHostedService
            ?? new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="config"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config,
        Func<IServiceProvider, TokenType> tokenType, Func<IServiceProvider, string> token,
        Func<IServiceProvider, bool>? validateToken = null)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, config);
        services.AddHostedService(provider => provider.GetRequiredService<TClient>() as IHostedService
            ?? new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(),
                tokenType(provider), token(provider), validateToken?.Invoke(provider) ?? true));
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="configure"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure,
        TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, configure);
        services.AddHostedService(provider => provider.GetRequiredService<TClient>() as IHostedService
            ?? new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(), tokenType, token, validateToken));
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="config"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <param name="tokenType"> The type of token to use. </param>
    /// <param name="token"> The token to use. </param>
    /// <param name="validateToken"> Whether to validate the token before logging in. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, TConfig, TClient> clientFactory, TConfig config,
        TokenType tokenType, string token, bool validateToken = true)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig
    {
        services.AddKookWebhookClient(clientFactory, config);
        services.AddHostedService(provider => provider.GetRequiredService<TClient>() as IHostedService
            ?? new KookClientHostedService<TClient>(provider.GetRequiredService<TClient>(), tokenType, token, validateToken));
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="configure"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {
        services.Configure(configure);
        services.AddKookWebhookClient(clientFactory);
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <param name="config"> The action to configure the <see cref="KookWebhookConfig"/>. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
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
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient, TConfig>(this IServiceCollection services,
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory)
        where TClient: KookWebhookClient
        where TConfig: KookWebhookConfig
    {
        services.AddSingleton<TClient>(provider => clientFactory(provider, provider.GetRequiredService<IOptions<TConfig>>()));
        services.AddKookWebhookClient<TClient>();
        return services;
    }

    /// <summary>
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <typeparam name="TConfig"> The type of the <see cref="KookWebhookConfig"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
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
    ///     Adds a <see cref="KookWebhookClient"/> to the specified <see cref="IServiceCollection"/> with
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add the services. </param>
    /// <param name="clientFactory"> The factory to create the <see cref="KookWebhookClient"/>. </param>
    /// <typeparam name="TClient"> The type of the <see cref="KookWebhookClient"/>. </typeparam>
    /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddKookWebhookClient<TClient>(this IServiceCollection services,
        Func<IServiceProvider, TClient> clientFactory)
        where TClient: KookWebhookClient
    {
        services.AddSingleton(clientFactory);
        services.AddKookWebhookClient<TClient>();
        return services;
    }

    private static void AddKookWebhookClient<TClient>(this IServiceCollection services)
        where TClient: KookWebhookClient
    {
        services.AddSingleton<IKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookWebhookClient, TClient>(provider => provider.GetRequiredService<TClient>());
    }
}
