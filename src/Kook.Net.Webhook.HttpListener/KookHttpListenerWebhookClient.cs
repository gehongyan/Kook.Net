namespace Kook.Webhook.HttpListener;

/// <summary>
///     Represents a KOOK webhook client using HTTP listener.
/// </summary>
public class KookHttpListenerWebhookClient : KookWebhookClient
{
    /// <inheritdoc />
    public KookHttpListenerWebhookClient(KookHttpListenerWebhookConfig config)
        : base(config)
    {
    }
}
