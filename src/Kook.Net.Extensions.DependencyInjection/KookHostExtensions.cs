using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kook.Net.DependencyInjection;

/// <summary>
///     Provides extension methods for Kook.Net using the <see cref="IHost"/>.
/// </summary>
public static class KookDependencyInjectionExtensions
{
    /// <summary>
    ///     Adds Kook service to <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKook(this IServiceCollection services)
    {
        return services;
    }
}
