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
}
