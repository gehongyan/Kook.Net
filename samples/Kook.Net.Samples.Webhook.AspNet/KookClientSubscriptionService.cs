using Kook.Webhook;

namespace Kook.Net.Samples.Webhook.AspNet;

public class KookClientSubscriptionService : IHostedService
{
    private readonly ILogger<KookClientSubscriptionService> _logger;

    public KookClientSubscriptionService(IServiceProvider provider, params ReadOnlySpan<string> clientNames)
    {
        _logger = provider.GetRequiredService<ILogger<KookClientSubscriptionService>>();
        foreach (string clientName in clientNames)
        {
            KookWebhookClient client = provider.GetRequiredKeyedService<KookWebhookClient>(clientName);
            SubscribeToEvents(clientName, client);
        }
    }

    private void SubscribeToEvents(string clientName, KookWebhookClient client)
    {
        client.Log += message =>
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
            }, message.Exception, "Kook.Webhook [{ClientName}] {Message}", clientName, message.Message);
            return Task.CompletedTask;
        };
        client.MessageReceived += (message, author, channel) =>
        {
            _logger.LogInformation("Message received [{ClientName}] {Message}", clientName, message);
            return Task.CompletedTask;
        };
        client.Ready += () =>
        {
            _logger.LogInformation("Kook.Webhook [{ClientName}] {CurrentUser} is ready!",
                clientName, client.CurrentUser?.UsernameAndIdentifyNumber(false));
            return Task.CompletedTask;
        };
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
