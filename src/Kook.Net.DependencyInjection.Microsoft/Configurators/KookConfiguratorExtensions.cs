using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     提供用于向 <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection"/> 添加 KOOK 客户端和服务的扩展方法。
/// </summary>
public static class KookConfiguratorExtensions
{
    /// <summary>
    ///     向指定的 <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> 添加 KOOK 客户端和服务。
    /// </summary>
    /// <param name="services"> 要向其添加 KOOK 客户端和服务的服务集合。 </param>
    /// <param name="configure"> 用于配置 KOOK 客户端和服务的配置委托。 </param>
    /// <returns> 添加了 KOOK 客户端和服务的服务集合。 </returns>
    public static IServiceCollection AddKook(this IServiceCollection services, Action<IKookClientServiceConfigurator> configure)
    {
        KookClientServiceConfigurator configurator = new(services);
        configure(configurator);
        configurator.Complete();
        return services;
    }
}
