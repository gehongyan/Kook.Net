using Kook.API;
using Kook.Net.Webhooks.HttpListener;

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

    /// <summary>
    ///     Gets the configuration used by this client.
    /// </summary>
    internal new KookHttpListenerWebhookConfig BaseConfig => base.BaseConfig as KookHttpListenerWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration for HTTP listener.");

    /// <inheritdoc />
    public override async Task StartAsync()
    {
        if (ApiClient.WebhookClient is not IHttpListenerWebhookClient httpListenerWebhookClient)
            throw new InvalidOperationException("The API client is not an HTTP listener-based client.");
        if (BaseConfig.UriPrefixes is not { Count: > 0 })
            throw new InvalidOperationException("The URI prefixes are not provided.");
        await base.StartAsync();
        await httpListenerWebhookClient.StartAsync(BaseConfig.UriPrefixes);
    }

    /// <inheritdoc />
    public override async Task StopAsync()
    {
        if (ApiClient.WebhookClient is not IHttpListenerWebhookClient httpListenerWebhookClient)
            throw new InvalidOperationException("The API client is not an HTTP listener-based client.");
        await httpListenerWebhookClient.StopAsync();
        await base.StopAsync();
    }
}
