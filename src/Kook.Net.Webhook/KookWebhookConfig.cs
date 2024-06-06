using Kook.Net.Webhooks;
using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client configuration.
/// </summary>
public abstract class KookWebhookConfig : KookSocketConfig
{
    /// <summary>
    ///     Gets or sets the verification token used to verify the webhook request.
    /// </summary>
    public string? VerifyToken { get; set; }

    /// <summary>
    ///     Gets or sets the encryption key used to decrypt the webhook payload.
    /// </summary>
    public string? EncryptKey { get; set; }

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
    /// <param name="webhookProvider"> The provider used to generate new UDP sockets. </param>
    protected KookWebhookConfig(WebhookProvider webhookProvider)
    {
        WebhookProvider = webhookProvider;
    }
}
