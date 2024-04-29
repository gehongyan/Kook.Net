using Kook.Net.Samples.ReactionRoleBot.Configurations;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Kook.Net.Samples.ReactionRoleBot.Extensions;

public partial class KookBotClientExtension : IHostedService
{
    private readonly KookBotConfigurations _kookBotConfigurations;
    private readonly KookSocketClient _kookSocketClient;
    private readonly ILogger _logger;

    public KookBotClientExtension(
        KookBotConfigurations kookBotConfigurations,
        KookSocketClient kookSocketClient,
        ILogger logger)
    {
        _kookBotConfigurations = kookBotConfigurations;
        _kookSocketClient = kookSocketClient;
        _logger = logger;

        _kookSocketClient.Log += LogHandler;
        _kookSocketClient.ReactionAdded += ProcessReactionRoleAdd;
        _kookSocketClient.ReactionRemoved += ProcessReactionRoleRemove;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _kookSocketClient.LoginAsync(TokenType.Bot, _kookBotConfigurations.Token);
        await _kookSocketClient.StartAsync();
    }

    /// <summary>
    ///     Kook.Net日志事件处理程序
    /// </summary>
    private Task LogHandler(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
                _logger.Fatal("Kook {Message:l}", msg.ToString());
                break;
            case LogSeverity.Error:
                _logger.Error("Kook {Message:l}", msg.ToString());
                break;
            case LogSeverity.Warning:
                _logger.Warning("Kook {Message:l}", msg.ToString());
                break;
            case LogSeverity.Info:
                _logger.Information("Kook {Message:l}", msg.ToString());
                break;
            case LogSeverity.Verbose:
                _logger.Verbose("Kook {Message:l}", msg.ToString());
                break;
            case LogSeverity.Debug:
                _logger.Debug("Kook {Message:l}", msg.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(LogSeverity));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Fatal("Kook Client Stopped!");
        return Task.CompletedTask;
    }
}
