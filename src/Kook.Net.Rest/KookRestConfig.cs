using Kook.Net.Rest;

namespace Kook.Rest;

/// <summary>
///     定义 Kook.Net 有关 REST 各种行为的配置类。
/// </summary>
/// <remarks>
///     此配置基于 <see cref="Kook.KookConfig"/>，在基础配置的基础上，定义了有关 REST 的配置。
/// </remarks>
public class KookRestConfig : KookConfig
{
    /// <summary>
    ///     获取或设置要用于创建 REST 客户端的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
