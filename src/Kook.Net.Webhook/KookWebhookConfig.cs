using Kook.Net.Webhooks;
using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client configuration.
/// </summary>
public class KookWebhookConfig : KookSocketConfig
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
    ///     Gets or sets the verification token used to verify the webhook request.
    /// </summary>
    public string? VerifyToken { get; set; }

    /// <summary>
    ///     Gets or sets the encryption key used to decrypt the webhook payload.
    /// </summary>
    public string? EncryptKey { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the token should be validated before logging in.
    /// </summary>
    public bool ValidateToken { get; set; } = true;

    /// <summary>
    ///     Gets or sets the route endpoint for the webhook.
    /// </summary>
    public string RouteEndpoint { get; set; } = "kook";

    /// <summary>
    ///     Gets or sets a function that is called when the webhook client is being configured.
    /// </summary>
    public Action<IServiceProvider, KookWebhookClient>? ConfigureKookClient { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the client should wait for a webhook challenge before connecting.
    /// </summary>
    public bool StartupWaitForChallenge { get; set; }

    /// <summary>
    ///     Gets or sets the provider used to generate new UDP sockets.
    /// </summary>
    public WebhookProvider WebhookProvider { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookWebhookConfig"/> class.
    /// </summary>
    public KookWebhookConfig()
    {
        WebhookProvider = DefaultWebhookProvider.Instance;
    }
}
