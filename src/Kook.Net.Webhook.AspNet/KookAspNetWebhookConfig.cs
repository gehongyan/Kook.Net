using Kook.Net.Webhooks.AspNet;

namespace Kook.Webhook.AspNet;

/// <summary>
///     表示一个用于 <see cref="Kook.Webhook.AspNet.KookAspNetWebhookClient"/> 的配置类。
/// </summary>
/// <remarks>
///     此配置基于 <see cref="Kook.Webhook.KookWebhookConfig"/>，在与 Webhook 有关的配置的基础上，定义了有关在 ASP.NET 内继承 KOOK Webhook 的配置。
/// </remarks>
public class KookAspNetWebhookConfig : KookWebhookConfig
{
    /// <summary>
    ///     获取或设置用于验证 KOOK API 的令牌类型。
    /// </summary>
    public TokenType? TokenType { get; set; }

    /// <summary>
    ///     获取或设置用于验证 KOOK API 的令牌。
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    ///     获取或设置是否在登录前应验证令牌。
    /// </summary>
    public bool ValidateToken { get; set; } = true;

    /// <summary>
    ///     获取或设置注册到 ASP.NET 的 KOOK Webhook 的路由模式。
    /// </summary>
    public string RoutePattern { get; set; } = "kook";

    /// <summary>
    ///     初始化一个 <see cref="KookAspNetWebhookConfig"/> 类的新实例。
    /// </summary>
    public KookAspNetWebhookConfig()
        : base(DefaultAspNetWebhookProvider.Instance)
    {
    }
}
