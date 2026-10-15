using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Services;

public class KookClientService : IHostedService
{
    private readonly KookSocketClient _socketClient;
    private readonly WebRtcSampleOptions _options;

    public KookClientService(KookSocketClient socketClient, WebRtcSampleOptions options, ILogger<KookClientService> logger)
    {
        _socketClient = socketClient;
        _options = options;
        socketClient.Log += message =>
        {
            logger.Log(ToLogLevel(message.Severity), message.Exception,
                "[Log] {Message} [Severity] {Severity} [Source] {Source}",
                message.Message ?? message.Exception?.Message ?? string.Empty, message.Severity, message.Source);
            return Task.CompletedTask;
        };
        socketClient.Connected += () =>
        {
            logger.LogInformation("[GatewayConnected] The gateway connected");
            return Task.CompletedTask;
        };
        socketClient.Disconnected += ex =>
        {
            logger.LogWarning("[GatewayDisconnected] The gateway disconnected [Error] {Error}", ex.Message);
            return Task.CompletedTask;
        };
        socketClient.Ready += () =>
        {
            logger.LogInformation("[Ready] The client is ready [Current User] {CurrentUser} [Guilds] {Guilds}",
                socketClient.CurrentUser?.Username, socketClient.Guilds.Count);
            return Task.CompletedTask;
        };
        socketClient.UserConnected += (user, channel, time) =>
        {
            logger.LogInformation("[UserConnected] A user joined a voice channel [User ID] {UserId} [Channel ID] {ChannelId} [Time] {Time}",
                user.Id, channel.Id, time);
            return Task.CompletedTask;
        };
        socketClient.UserDisconnected += (user, channel, time) =>
        {
            logger.LogInformation("[UserDisconnected] A user left a voice channel [User ID] {UserId} [Channel ID] {ChannelId} [Time] {Time}",
                user.Id, channel.Id, time);
            return Task.CompletedTask;
        };
    }

    private static LogLevel ToLogLevel(LogSeverity severity) => severity switch
    {
        LogSeverity.Critical => LogLevel.Critical,
        LogSeverity.Error => LogLevel.Error,
        LogSeverity.Warning => LogLevel.Warning,
        LogSeverity.Info => LogLevel.Information,
        LogSeverity.Verbose => LogLevel.Debug,
        LogSeverity.Debug => LogLevel.Debug,
        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
    };

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 令牌从文件或环境变量读取，不要把它写进代码里
        string token = _options.ResolveBotToken();
        await _socketClient.LoginAsync(TokenType.Bot, token);
        await _socketClient.StartAsync();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _socketClient.StopAsync();
        await _socketClient.LogoutAsync();
    }
}
