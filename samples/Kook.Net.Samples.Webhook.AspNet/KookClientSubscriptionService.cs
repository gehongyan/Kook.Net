using Kook.Webhook;

namespace Kook.Net.Samples.Webhook.AspNet;

public class KookClientSubscriptionService : IHostedService
{
    private readonly ILogger<KookClientSubscriptionService> _logger;
    private readonly KookWebhookClient _kookWebhookClient;

    public KookClientSubscriptionService(ILogger<KookClientSubscriptionService> logger, KookWebhookClient kookWebhookClient)
    {
        _logger = logger;
        _kookWebhookClient = kookWebhookClient;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _kookWebhookClient.Log += message =>
        {
            _logger.Log(message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => throw new ArgumentOutOfRangeException(nameof(message.Severity), message.Severity, null)
            }, message.Exception, "Kook.Webhook: {Message}", message.Message);
            return Task.CompletedTask;
        };
        _kookWebhookClient.MessageReceived += (message, author, channel) =>
        {
            _logger.LogInformation("Message received: {Message}", message);
            return Task.CompletedTask;
        };
        _kookWebhookClient.Ready += () =>
        {
            _logger.LogInformation("Ready!");
            return Task.CompletedTask;
        };
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
