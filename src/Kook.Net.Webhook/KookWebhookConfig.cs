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
    ///     Gets or sets a value indicating whether the client should try to automatically log in.
    /// </summary>
    public bool AutoLogin { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the client should try to automatically log out.
    /// </summary>
    public bool AutoLogout { get; set; } = false;

    /// <summary>
    ///     Gets or sets the provider used to generate new UDP sockets.
    /// </summary>
    public WebhookProvider WebhookProvider { get; set; }

    /// <summary>
    ///     Gets the heartbeat interval of WebSocket connection in milliseconds.
    /// </summary>
    public new int HeartbeatIntervalMilliseconds
    {
        get => base.HeartbeatIntervalMilliseconds;
        set => base.HeartbeatIntervalMilliseconds = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookWebhookConfig"/> class.
    /// </summary>
    /// <param name="webhookProvider"> The provider used to generate new UDP sockets. </param>
    protected KookWebhookConfig(WebhookProvider webhookProvider)
    {
        WebhookProvider = webhookProvider;
        HandlerTimeout = 1000;
        base.HeartbeatIntervalMilliseconds = 60000;
    }
}
