using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace Kook.Net.Samples.CardMarkup.Services;

public class KookSocketService : IHostedService
{
    private readonly KookSocketClient _socketClient;
    private readonly CommandHandlerService _commandHandler;
    private readonly ILogger<KookSocketService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public KookSocketService(
        KookSocketClient socketClient,
        CommandHandlerService commandHandler,
        ILogger<KookSocketService> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _socketClient = socketClient;
        _commandHandler = commandHandler;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _socketClient.Log += LogMessage;
        _socketClient.Ready += async () =>
        {
            _logger.LogInformation("KookSocketClient is ready");
            await Task.CompletedTask;
        };
        _socketClient.Disconnected += async e  =>
        {
            _logger.LogCritical(e, "KookSocketClient is disconnected");
            _applicationLifetime.StopApplication();
            await Task.CompletedTask;
        };

        var token = Environment.GetEnvironmentVariable("KookDebugToken");
        await _socketClient.LoginAsync(TokenType.Bot, token);

        await _socketClient.StartAsync();
        await _commandHandler.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _socketClient.StopAsync();
    }

    private Task LogMessage(LogMessage message)
    {
        _logger.LogDebug("Message received: {Message}", message.ToString());
        return Task.CompletedTask;
    }
}
