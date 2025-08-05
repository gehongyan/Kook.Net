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
    ///     在登录前是否先退出登录。
    /// </summary>
    /// <remarks>
    ///     如果为 <see langword="true"/>，则在调用登陆方法时会向 KOOK API
    ///     调用退出登录接口，这会使所有使用与传入该登陆方法相同的登录信息的 Bot 客户端的网关连接离线。
    ///     <br />
    ///     KOOK 不支持多 Bot 客户端同时保持连接，当已有 Bot 客户端在线时，后续的客户端再建立的连接虽可以握手成功，但无法收到任何来自
    ///     KOOK 网关下发的业务事件，Bot 客户端会因此陷入“假活而不自知”的状态。设置此属性为 <c>true</c> 时，可以确保当前 Bot
    ///     客户端新实例连接至网关时，能够接收到来自 KOOK 网关的业务事件。
    ///     <br />
    ///     <note type="warning">
    ///         设置此属性为 <c>true</c> 时，会导致与此客户端使用相同登录信息的 Bot
    ///         客户端现有的网关连接无法再收到任何业务事件，也不会被 KOOK 网关主动断开连接，从而陷入“假活而不自知”的状态。
    ///     </note>
    /// </remarks>
    public bool AutoLogoutBeforeLogin { get; set; }

    /// <summary>
    ///     获取或设置要用于创建 REST 客户端的 <see cref="Kook.Net.Rest.RestClientProvider"/> 委托。
    /// </summary>
    public RestClientProvider RestClientProvider { get; set; } = DefaultRestClientProvider.Instance;
}
