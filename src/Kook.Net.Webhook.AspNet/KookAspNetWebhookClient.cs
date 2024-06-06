using Kook.API;
using Kook.Net.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook.AspNet;

/// <summary>
///     Represents a KOOK webhook client using ASP.NET.
/// </summary>
public class KookAspNetWebhookClient : KookWebhookClient, IHostedService
{
    private readonly KookAspNetWebhookConfig _config;

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookAspNetWebhookClient" /> class.
    /// </summary>
    /// <param name="serviceProvider"> The <see cref="IServiceProvider" /> to resolve services from. </param>
    /// <param name="config"> The <see cref="IOptions{TOptions}" /> to configure the KOOK webhook client with. </param>
    public KookAspNetWebhookClient(IServiceProvider serviceProvider, IOptions<KookAspNetWebhookConfig> config)
        : base(config.Value, CreateApiClient(serviceProvider, config.Value))
    {
        _config = config.Value;
        config.Value.ConfigureKookClient?.Invoke(serviceProvider, this);
    }

    private static KookWebhookApiClient CreateApiClient(IServiceProvider serviceProvider, KookWebhookConfig config)
    {
        if (config.EncryptKey is null)
            throw new InvalidOperationException("Encryption key is required.");
        if (config.VerifyToken is null)
            throw new InvalidOperationException("Verify token is required.");
        WebhookProvider webhookProvider = config.WebhookProvider ?? serviceProvider.GetRequiredService<WebhookProvider>();
        return new KookWebhookApiClient(config.RestClientProvider, config.WebSocketProvider, webhookProvider,
            config.EncryptKey, config.VerifyToken, KookConfig.UserAgent, config.AcceptLanguage,
            defaultRatelimitCallback: config.DefaultRatelimitCallback);
    }

    /// <inheritdoc />
    public override Task StartAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    public override Task StopAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (!_config.TokenType.HasValue)
            throw new InvalidOperationException("The KOOK webhook client token type is not available.");
        if (_config.Token is null)
            throw new InvalidOperationException("The KOOK webhook client token is not available.");
        await LoginAsync(_config.TokenType.Value, _config.Token, _config.ValidateToken);
        await base.StartAsync();
    }

    /// <inheritdoc />
    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync();
        await LogoutAsync();
    }
}
