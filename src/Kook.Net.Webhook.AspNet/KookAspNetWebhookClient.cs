using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook.AspNet;

/// <summary>
///     Represents a KOOK webhook client using ASP.NET.
/// </summary>
public class KookAspNetWebhookClient : KookWebhookClient, IHostedService
{
    /// <inheritdoc />
    public KookAspNetWebhookClient(IServiceProvider serviceProvider, IOptions<KookWebhookConfig> config)
        : base(serviceProvider, config.Value)
    {
    }

    /// <inheritdoc />
    public override Task StartAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    public override Task StopAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    Task IHostedService.StartAsync(CancellationToken cancellationToken) => base.StartAsync();

    /// <inheritdoc />
    Task IHostedService.StopAsync(CancellationToken cancellationToken) => base.StopAsync();
}
