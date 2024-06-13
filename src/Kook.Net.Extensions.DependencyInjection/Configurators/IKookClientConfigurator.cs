using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Represents a configurator for a Kook client.
/// </summary>
/// <typeparam name="TClient"> The type of the client. </typeparam>
/// <typeparam name="TConfig"> The type of the configuration. </typeparam>
public interface IKookClientConfigurator<TClient, TConfig>
    where TClient : IKookClient
    where TConfig : KookConfig
{
    /// <summary>
    ///     Gets the service collection to append services.
    /// </summary>
    IServiceCollection ServiceCollection { get; }

    /// <summary>
    ///     Appends a configuration action, which is executed on the created configuration instance.
    /// </summary>
    /// <param name="configure"> The configuration action. </param>
    /// <returns> The configurator. </returns>
    IKookClientConfigurator<TClient, TConfig> AppendConfigure(Action<IServiceProvider, TConfig> configure);

    /// <summary>
    ///     Appends a service action, which is executed on the service collection.
    /// </summary>
    /// <param name="service"> The service action. </param>
    /// <returns> The configurator. </returns>
    IKookClientConfigurator<TClient, TConfig> AppendService(Action<IServiceCollection> service);
}
