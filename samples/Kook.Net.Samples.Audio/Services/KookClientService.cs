using Kook.Commands;
using Kook.WebSocket;

namespace Kook.Net.Samples.Audio.Services;

public class KookClientService : IHostedService
{
    private readonly KookSocketClient _socketClient;

    public KookClientService(KookSocketClient socketClient,
        CommandService commandService)
    {
        _socketClient = socketClient;
        socketClient.Log += LogAsync;
        commandService.Log += LogAsync;
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User);
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
