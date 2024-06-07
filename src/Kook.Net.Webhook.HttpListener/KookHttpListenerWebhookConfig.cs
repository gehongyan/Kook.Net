using Kook.Net.Webhooks.HttpListener;

namespace Kook.Webhook.HttpListener;

/// <summary>
///     Represents a KOOK webhook client configuration using HTTP listener.
/// </summary>
public class KookHttpListenerWebhookConfig : KookWebhookConfig
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KookHttpListenerWebhookConfig"/> class.
    /// </summary>
    public KookHttpListenerWebhookConfig()
        : base(DefaultHttpListenerWebhookProvider.Instance)
    {
    }

    /// <summary>
    ///     Gets or sets the URI prefixes to listen for incoming webhook requests.
    /// </summary>
    public IReadOnlyCollection<string>? UriPrefixes { get; set; }
}
