using Kook.Net.Webhooks.AspNet;
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
        : base(config.Value)
    {
        _config = config.Value;
        config.Value.ConfigureKookClient?.Invoke(serviceProvider, this);
    }

    /// <summary>
    ///     Gets the configuration used by this client.
    /// </summary>
    internal new KookAspNetWebhookConfig BaseConfig => base.BaseConfig as KookAspNetWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration for ASP.NET.");

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
        if (ApiClient.WebhookClient is AspNetWebhookClient aspNetWebhookClient)
            aspNetWebhookClient.OnClosed(new OperationCanceledException("The hosted service has been stopped."));
    }
}
