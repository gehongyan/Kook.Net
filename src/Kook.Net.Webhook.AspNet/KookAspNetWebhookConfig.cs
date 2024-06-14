using Kook.Net.Webhooks.AspNet;

namespace Kook.Webhook.AspNet;

/// <summary>
///     Represents a KOOK webhook client configuration for ASP.NET.
/// </summary>
public class KookAspNetWebhookConfig : KookWebhookConfig
{
    /// <summary>
    ///     Gets or sets the token type used to authenticate with the KOOK API.
    /// </summary>
    public TokenType? TokenType { get; set; }

    /// <summary>
    ///     Gets or sets the token used to authenticate with the KOOK API.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the token should be validated before logging in.
    /// </summary>
    public bool ValidateToken { get; set; } = true;

    /// <summary>
    ///     Gets or sets the route endpoint for the webhook.
    /// </summary>
    public string RoutePattern { get; set; } = "kook";

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookAspNetWebhookConfig"/> class.
    /// </summary>
    public KookAspNetWebhookConfig()
        : base(DefaultAspNetWebhookProvider.Instance)
    {
    }
}
