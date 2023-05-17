using Kook;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TextCommandFramework.Services;

public class CommandHandlingService
{
    private readonly CommandService _commands;
    private readonly KookSocketClient _kook;
    private readonly IServiceProvider _services;

    public CommandHandlingService(IServiceProvider services)
    {
        _commands = services.GetRequiredService<CommandService>();
        _kook = services.GetRequiredService<KookSocketClient>();
        _services = services;

        // Hook CommandExecuted to handle post-command-execution logic.
        _commands.CommandExecuted += CommandExecutedAsync;
        // Hook MessageReceived so we can process each message to see
        // if it qualifies as a command.
        _kook.MessageReceived += MessageReceivedAsync;
        _kook.DirectMessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync() =>
        // Register modules that are public and inherit ModuleBase<T>.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

    public async Task MessageReceivedAsync(SocketMessage rawMessage, SocketUser user, ISocketMessageChannel channel)
    {
        // Ignore system messages, or messages from other bots
        if (rawMessage is not SocketUserMessage {Source: MessageSource.User} message) return;

        // This value holds the offset where the prefix ends
        int argPos = 0;
        // Perform prefix check. You may want to replace this with
        if (!message.HasCharPrefix('!', ref argPos)) return;
        // for a more traditional command format like !help.
        // if (!message.HasMentionPrefix(_kook.CurrentUser, ref argPos))
        //     return;

        SocketCommandContext context = new(_kook, message);
        // Perform the execution of the command. In this method,
        // the command service will perform precondition and parsing check
        // then execute the command if one is matched.
        await _commands.ExecuteAsync(context, argPos, _services);
        // Note that normally a result will be returned by this format, but here
        // we will handle the result in CommandExecutedAsync,
    }

    public async Task CommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result)
    {
        // command is unspecified when there was a search failure (command not found); we don't care about these errors
        if (command is null) return;

        // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
        if (result.IsSuccess) return;

        // the command failed, let's notify the user that something happened.
        await context.Channel.SendTextAsync($"error: {result}");
    }
}
