using Kook.Net.Webhooks.AspNet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook.AspNet;

/// <summary>
///     表示一个使用 ASP.NET 的 KOOK Webhook 客户端。
/// </summary>
public class KookAspNetWebhookClient : KookWebhookClient, IHostedService
{
    internal KookAspNetWebhookClient(IOptions<KookAspNetWebhookConfig> config)
        : base(config.Value)
    {
    }

    internal KookAspNetWebhookClient(KookAspNetWebhookConfig config)
        : base(config)
    {
    }

    internal new KookAspNetWebhookConfig BaseConfig => base.BaseConfig as KookAspNetWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration for ASP.NET.");

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> KOOK Webhook 客户端不支持手动启动。 </exception>
    public override Task StartAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> KOOK Webhook 客户端不支持手动停止。 </exception>
    public override Task StopAsync() =>
        throw new NotSupportedException("Webhook client does not support stopping manually.");

    /// <inheritdoc />
    protected override async Task OnWebhookChallengeAsync(string challenge)
    {
        if (!BaseConfig.AutoLogin && ConnectionState == ConnectionState.Disconnected)
            await StartCoreAsync();
        await base.OnWebhookChallengeAsync(challenge);
    }

    /// <inheritdoc />
    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (BaseConfig.AutoLogin)
            await StartCoreAsync();
    }

    private async Task StartCoreAsync()
    {
        if (!BaseConfig.TokenType.HasValue)
            throw new InvalidOperationException("The KOOK webhook client token type is not available.");
        if (BaseConfig.Token is null)
            throw new InvalidOperationException("The KOOK webhook client token is not available.");
        await LoginAsync(BaseConfig.TokenType.Value, BaseConfig.Token, BaseConfig.ValidateToken);
        await base.StartAsync();
    }

    /// <inheritdoc />
    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync();
        if (BaseConfig.AutoLogout)
            await LogoutAsync();
        if (ApiClient.WebhookClient is AspNetWebhookClient aspNetWebhookClient)
            aspNetWebhookClient.OnClosed(new OperationCanceledException("The hosted service has been stopped."));
    }
}
