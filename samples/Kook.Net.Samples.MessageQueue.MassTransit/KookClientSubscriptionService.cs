using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.MessageQueue.MassTransit;

public class KookClientSubscriptionService : IHostedService
{
    private readonly ILogger<KookClientSubscriptionService> _logger;
    private readonly KookSocketClient _kookSocketClient;

    public KookClientSubscriptionService(ILogger<KookClientSubscriptionService> logger, KookSocketClient kookSocketClient)
    {
        _logger = logger;
        _kookSocketClient = kookSocketClient;
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _kookSocketClient.Log += message =>
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
        _kookSocketClient.Ready += () =>
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
