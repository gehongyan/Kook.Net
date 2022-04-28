using KaiHeiLa.Net.Samples.ApiHelper.Configurations;
using KaiHeiLa.WebSocket;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KaiHeiLa.Net.Samples.ApiHelper.Extensions;

public partial class KaiHeiLaBotClientExtension : IHostedService
{
    private readonly IServiceProvider _service;
    private readonly KaiHeiLaBotConfigurations _kaiHeiLaBotConfigurations;
    private readonly KaiHeiLaSocketClient _kaiHeiLaSocketClient;
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public KaiHeiLaBotClientExtension(
        IServiceProvider service,
        KaiHeiLaBotConfigurations kaiHeiLaBotConfigurations,
        KaiHeiLaSocketClient kaiHeiLaSocketClient,
        ILogger logger, IHttpClientFactory httpClientFactory)
    {
        _service = service;
        _kaiHeiLaBotConfigurations = kaiHeiLaBotConfigurations;
        _kaiHeiLaSocketClient = kaiHeiLaSocketClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;

        _kaiHeiLaSocketClient.Log += LogHandler;
        _kaiHeiLaSocketClient.MessageReceived += async message => await ProcessApiHelperCommand(message);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _kaiHeiLaSocketClient.LoginAsync(TokenType.Bot, "_kaiHeiLaBotConfigurations.Token");
        await _kaiHeiLaSocketClient.StartAsync();
    }

    /// <summary>
    ///     KaiHeiLa.Net日志事件处理程序
    /// </summary>
    private Task LogHandler(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
                _logger.Fatal("KaiHeiLa {Message:l}", msg.ToString());
                break;
            case LogSeverity.Error:
                _logger.Error("KaiHeiLa {Message:l}", msg.ToString());
                break;
            case LogSeverity.Warning:
                _logger.Warning("KaiHeiLa {Message:l}", msg.ToString());
                break;
            case LogSeverity.Info:
                _logger.Information("KaiHeiLa {Message:l}", msg.ToString());
                break;
            case LogSeverity.Verbose:
                _logger.Verbose("KaiHeiLa {Message:l}", msg.ToString());
                break;
            case LogSeverity.Debug:
                _logger.Debug("KaiHeiLa {Message:l}", msg.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(LogSeverity));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Fatal("KaiHeiLa Client Stopped!");
        return Task.CompletedTask;
    }
}