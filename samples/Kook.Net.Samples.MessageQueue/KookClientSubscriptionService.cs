using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.MessageQueue.MassTransit;

public class KookClientSubscriptionService : IHostedService
{
    private readonly ILogger<KookClientSubscriptionService> _logger;
    private readonly KookSocketClient _kookClient;

    public KookClientSubscriptionService(ILogger<KookClientSubscriptionService> logger, KookSocketClient kookClient)
    {
        _logger = logger;
        _kookClient = kookClient;
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _kookClient.Log += message =>
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
            }, message.Exception, "Kook: {Message}", message.Message);
            return Task.CompletedTask;
        };
        _kookClient.MessageReceived += (message, author, channel) =>
        {
            _logger.LogInformation("Message received: {Message}", message);
            return Task.CompletedTask;
        };
        _kookClient.Ready += () =>
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
