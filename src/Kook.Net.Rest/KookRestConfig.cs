using Kook.Net.Rest;

namespace Kook.Rest;

/// <summary>
///     定义 Kook.Net 有关 REST 各种行为的配置类。
/// </summary>
public class KookRestConfig : KookConfig
{
    /// <summary>
    ///     获取或设置要用于创建 REST 客户端的 <see cref="T:Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
