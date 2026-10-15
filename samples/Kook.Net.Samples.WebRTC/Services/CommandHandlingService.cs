using System.Reflection;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Services;

public class CommandHandlingService : IHostedService
{
    private readonly CommandService _commands;
    private readonly KookSocketClient _kook;
    private readonly IServiceProvider _services;
    private readonly WebRtcSampleOptions _options;
    private readonly ILogger<CommandHandlingService> _logger;

    public CommandHandlingService(IServiceProvider services, CommandService commands, KookSocketClient kook,
        WebRtcSampleOptions options, ILogger<CommandHandlingService> logger)
    {
        _services = services;
        _commands = commands;
        _kook = kook;
        _options = options;
        _logger = logger;

        _commands.CommandExecuted += CommandExecutedAsync;
        _kook.MessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        // 注册所有公开且继承了 ModuleBase<T> 的模块
        if (Assembly.GetEntryAssembly() is not { } entryAssembly) return;
        await _commands.AddModulesAsync(entryAssembly, _services);
    }

    public async Task MessageReceivedAsync(SocketMessage rawMessage, SocketUser user, ISocketMessageChannel channel)
    {
        // 忽略系统消息和其他机器人的消息
        if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message) return;

        int argPos = 0;
        if (!message.HasCharPrefix(_options.CommandPrefix, ref argPos)) return;

        SocketCommandContext context = new(_kook, message);
        _logger.LogInformation("[Command] Received a command [User ID] {UserId} [Channel ID] {ChannelId} [Content] {Content}",
            user.Id, channel.Id, message.Content);
        await _commands.ExecuteAsync(context, argPos, _services);
    }

    public async Task CommandExecutedAsync(CommandInfo? command, ICommandContext context, Commands.IResult result)
    {
        // 未找到命令时 command 为 null，不需要处理
        if (command is null) return;

        if (result.IsSuccess)
        {
            _logger.LogInformation("[Command] Executed a command [Command] {Command}", command.Name);
            return;
        }

        _logger.LogWarning("[Command] A command failed [Command] {Command} [Error] {Error}", command.Name, result.ErrorReason);
        await context.Channel.SendTextAsync($"error: {result.ErrorReason}");
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => InitializeAsync();

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
