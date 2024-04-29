using System.Reflection;
using Kook.Commands;
using Kook.WebSocket;

namespace Kook.Net.Samples.CardMarkup.Services;

public class CommandHandlerService
{
    private readonly CommandService _commandService;
    private readonly KookSocketClient _socketClient;
    private readonly IServiceProvider _serviceProvider;

    public CommandHandlerService(CommandService commandService, KookSocketClient socketClient, IServiceProvider serviceProvider)
    {
        _commandService = commandService;
        _socketClient = socketClient;
        _serviceProvider = serviceProvider;

        _commandService.CommandExecuted += CommandExecutedAsync;
        _socketClient.MessageReceived += MessageReceivedAsync;
        _socketClient.DirectMessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        if (Assembly.GetEntryAssembly() is not { } assembly) return;
        await _commandService.AddModulesAsync(assembly, _serviceProvider);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage, SocketUser user, ISocketMessageChannel channel)
    {
        if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message) return;

        int argPos = 0;
        if (!message.HasCharPrefix('!', ref argPos)) return;

        SocketCommandContext context = new(_socketClient, message);
        await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
    }

    private static async Task CommandExecutedAsync(CommandInfo? command, ICommandContext context, IResult result)
    {
        if (result.IsSuccess) return;
        await context.Channel.SendTextAsync($"error: {result}");
    }
}
