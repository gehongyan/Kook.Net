using Kook.Net.Webhooks;
using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     表示一个用于 <see cref="T:Kook.Webhook.KookWebhookClient"/> 的配置类。
/// </summary>
/// <remarks>
///     此配置基于 <see cref="T:Kook.WebSocket.KookSocketConfig"/>，在与网关有关的配置的基础上，定义了有关 Webhook 的配置。
/// </remarks>
public abstract class KookWebhookConfig : KookSocketConfig
{
    /// <summary>
    ///     获取或设置用于验证 Webhook 请求的验证令牌。
    /// </summary>
    public string? VerifyToken { get; set; }

    /// <summary>
    ///     获取或设置用于解密 Webhook 负载的加密密钥。
    /// </summary>
    public string? EncryptKey { get; set; }

    /// <summary>
    ///     获取或设置客户端是否尝试自动登录。
    /// </summary>
    public bool AutoLogin { get; set; } = true;

    /// <summary>
    ///     获取或设置客户端是否尝试自动退出登录。
    /// </summary>
    public bool AutoLogout { get; set; } = false;

    /// <summary>
    ///     获取或设置用于创建 Webhook 客户端的委托。
    /// </summary>
    public WebhookProvider WebhookProvider { get; set; }

    /// <summary>
    ///     获取或设置网关发送心跳包的时间间隔（毫秒）。
    /// </summary>
    public new int HeartbeatIntervalMilliseconds
    {
        get => base.HeartbeatIntervalMilliseconds;
        set => base.HeartbeatIntervalMilliseconds = value;
    }

    /// <summary>
    ///     初始化一个 <see cref="KookWebhookConfig"/> 类的新实例。
    /// </summary>
    /// <param name="webhookProvider"> 用于创建 Webhook 客户端的委托。 </param>
    protected KookWebhookConfig(WebhookProvider webhookProvider)
    {
        WebhookProvider = webhookProvider;
        HandlerTimeout = 1000;
        base.HeartbeatIntervalMilliseconds = 60000;
    }
}
