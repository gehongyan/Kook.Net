using Kook.Net.Webhooks.HttpListener;

namespace Kook.Webhook.HttpListener;

/// <summary>
///     表示一个用于 <see cref="Kook.Webhook.HttpListener.KookHttpListenerWebhookClient"/> 的配置类。
/// </summary>
/// <remarks>
///     此配置基于 <see cref="Kook.Webhook.KookWebhookConfig"/>，在与 Webhook 有关的配置的基础上，定义了有关 HTTP 监听器的配置。
/// </remarks>
public class KookHttpListenerWebhookConfig : KookWebhookConfig
{
    /// <summary>
    ///     初始化一个 <see cref="KookHttpListenerWebhookConfig"/> 类的新实例。
    /// </summary>
    public KookHttpListenerWebhookConfig()
        : base(DefaultHttpListenerWebhookProvider.Instance)
    {
    }

    /// <summary>
    ///     获取或设置用于监听传入 Webhook 请求的 URI 前缀。
    /// </summary>
    public IReadOnlyCollection<string>? UriPrefixes { get; set; }

    /// <summary>
    ///     获取或设置在 HTTP 监听器关闭后等待重新启动的时间间隔。
    /// </summary>
    /// <remarks>
    ///     设置为与 <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>
    ///     相等的值表示客户端将在保持应用程序运行的情况下不重新启动；设置为其它任何负值将导致客户端在 HTTP 监听器关闭后退出应用程序；设置为
    ///     <see cref="System.TimeSpan.Zero"/> 将导致客户端在 HTTP
    ///     监听器关闭后立即重新启动；设置为任何正值将导致客户端在指定的时间间隔后重新启动。 <br />
    ///     默认值为 5 秒，即在 HTTP 监听器关闭后 5 秒后重新启动。
    /// </remarks>
    public TimeSpan AutoRestartInterval { get; set; } = TimeSpan.FromSeconds(5);
}
