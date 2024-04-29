using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Kook.Net.Samples.TextCommands.Services;

public class KookClientService : IHostedService
{
    private readonly KookSocketClient _client;
    private readonly CommandService _commandService;
    private readonly CommandHandlingService _commandHandlingService;

    public KookClientService(KookSocketClient client,
        CommandService commandService, CommandHandlingService commandHandlingService)
    {
        _client = client;
        _commandService = commandService;
        _commandHandlingService = commandHandlingService;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;
        _commandService.Log += LogAsync;

        // 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
        // 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
        // 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
        // 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
        string token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
            ?? throw new InvalidOperationException("Token not found");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _commandHandlingService.InitializeAsync();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    /// <summary>
    ///     Log 事件，此处以直接输出到控制台为例
    /// </summary>
    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
