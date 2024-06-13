using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Net.Extensions.DependencyInjection;

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

    internal static void AddKookWebhookClient<TClient>(this IServiceCollection services)
        where TClient: KookWebhookClient
    {
        services.AddSingleton<IKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseKookClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<BaseSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookSocketClient, TClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<KookWebhookClient, TClient>(provider => provider.GetRequiredService<TClient>());
    }
}
