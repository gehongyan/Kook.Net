using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for <see cref="IServiceCollection"/> to add Kook clients and services.
/// </summary>
public static class KookConfiguratorExtensions
{
    /// <summary>
    ///     Adds a Kook client to the service collection.
    /// </summary>
    /// <param name="services"> The service collection. </param>
    /// <param name="configure"> The configuration action. </param>
    /// <returns> The service collection. </returns>
    public static IServiceCollection AddKook(this IServiceCollection services, Action<IKookClientServiceConfigurator> configure)
    {
        IKookClientServiceConfigurator configurator = new KookClientServiceConfigurator(services);
        configure(configurator);
        configurator.Complete();
        return services;
    }
}
