using Microsoft.Extensions.DependencyInjection;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     表示一个通用的 KOOK 客户端配置器。
/// </summary>
/// <typeparam name="TClient"> 客户端的类型。 </typeparam>
/// <typeparam name="TConfig"> 配置的类型。 </typeparam>
public interface IKookClientConfigurator<TClient, TConfig>
    where TClient : IKookClient
    where TConfig : KookConfig
{
    /// <summary>
    ///     获取用于添加服务的服务集合。
    /// </summary>
    IServiceCollection ServiceCollection { get; }

    /// <summary>
    ///     添加一个配置操作，该操作将在创建的配置实例上执行。
    /// </summary>
    /// <param name="configure"> 配置操作。 </param>
    /// <returns> 添加了配置操作的配置器。 </returns>
    IKookClientConfigurator<TClient, TConfig> AppendConfigure(Action<IServiceProvider, TConfig> configure);

    /// <summary>
    ///     添加一个服务操作，该操作将在服务集合上执行。
    /// </summary>
    /// <param name="service"> 服务操作。 </param>
    /// <returns> 添加了服务操作的配置器。 </returns>
    IKookClientConfigurator<TClient, TConfig> AppendService(Action<IServiceCollection> service);
}
