using System.Diagnostics.CodeAnalysis;
using Kook.Rest;
using Kook.Webhook;
using Kook.WebSocket;
using Microsoft.Extensions.Options;

namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     表示一个通用的 KOOK 客户端服务配置器。
/// </summary>
public interface IKookClientServiceConfigurator : IKookClientConfiguratorCompleter
{
    /// <summary>
    ///     配置使用 REST 客户端。
    /// </summary>
    /// <param name="configure"> 配置操作。 </param>
    /// <returns> 配置了 REST 客户端的配置器。 </returns>
    IKookClientConfigurator<KookRestClient, KookRestConfig> UseRestClient(Action<KookRestConfig> configure);

    /// <summary>
    ///     配置使用网关客户端。
    /// </summary>
    /// <param name="configure"> 配置操作。 </param>
    /// <returns> 配置了网关客户端的配置器。 </returns>
    IKookClientConfigurator<KookSocketClient, KookSocketConfig> UseSocketClient(Action<KookSocketConfig> configure);

    /// <summary>
    ///     配置使用基于 Webhook 的网关客户端。
    /// </summary>
    /// <param name="clientFactory"> 客户端创建委托。 </param>
    /// <param name="configure"> 配置操作。 </param>
    /// <typeparam name="TClient"> 客户端的类型。 </typeparam>
    /// <typeparam name="TConfig"> 配置的类型。 </typeparam>
    /// <returns> 配置了基于 Webhook 的网关客户端的配置器。 </returns>
    IKookClientConfigurator<TClient, TConfig> UseWebhookClient<TClient,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TConfig>(
        Func<IServiceProvider, IOptions<TConfig>, TClient> clientFactory, Action<TConfig> configure)
        where TClient : KookWebhookClient
        where TConfig : KookWebhookConfig;
}
