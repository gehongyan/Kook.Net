using Kook.Net.Webhooks.HttpListener;

namespace Kook.Webhook.HttpListener;

/// <summary>
///     表示一个使用 HTTP 监听器的 KOOK Webhook 客户端。
/// </summary>
public class KookHttpListenerWebhookClient : KookWebhookClient
{
    /// <summary>
    ///     初始化一个 <see cref="KookHttpListenerWebhookClient"/> 类的新实例。
    /// </summary>
    /// <param name="config"> 用于配置 KOOK Webhook 客户端的 <see cref="T:Kook.Webhook.HttpListener.KookHttpListenerWebhookConfig"/>。 </param>
    public KookHttpListenerWebhookClient(KookHttpListenerWebhookConfig config)
        : base(config)
    {
    }

    internal new KookHttpListenerWebhookConfig BaseConfig => base.BaseConfig as KookHttpListenerWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration for HTTP listener.");

    /// <inheritdoc />
    public override async Task StartAsync()
    {
        if (ApiClient.WebhookClient is not IHttpListenerWebhookClient httpListenerWebhookClient)
            throw new InvalidOperationException("The API client is not an HTTP listener-based client.");
        if (BaseConfig.UriPrefixes is not { Count: > 0 })
            throw new InvalidOperationException("The URI prefixes are not provided.");
        await WebhookLogger.InfoAsync("Starting the KOOK webhook client...");
        await base.StartAsync();
        await httpListenerWebhookClient.StartAsync(BaseConfig.UriPrefixes);
        await WebhookLogger.InfoAsync("The KOOK webhook client has started.");
        httpListenerWebhookClient.Closed += async ex =>
        {
            await WebhookLogger.ErrorAsync("The HTTP listener has been closed.", ex);

            if (BaseConfig.AutoRestartInterval == Timeout.InfiniteTimeSpan)
            {
                await WebhookLogger.ErrorAsync("Shutting down the KOOK webhook client.");
                return;
            }

            if (BaseConfig.AutoRestartInterval < TimeSpan.Zero)
            {
                await WebhookLogger.ErrorAsync("Shutting down the KOOK webhook client and exiting the application.");
                Environment.Exit(1);
                return;
            }

            await WebhookLogger.InfoAsync($"Restarting the HTTP listener in {BaseConfig.AutoRestartInterval}...");
            await Task.Delay(BaseConfig.AutoRestartInterval);
            await WebhookLogger.InfoAsync("Restarting the HTTP listener...");
            await httpListenerWebhookClient.StartAsync(BaseConfig.UriPrefixes);
            await WebhookLogger.InfoAsync("The HTTP listener has been restarted.");
        };
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
