public async Task SetupAsync()
{
    await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    // 订阅命令执行后处理程序
    _commands.CommandExecuted += OnCommandExecutedAsync;
    // 订阅命令执行处理程序
    _client.MessageReceived += HandleCommandAsync;
}
public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
{
    // CommandExecuted 提供三个参数：所执行的命令、命令执行上下文、命令执行结果

    // 可以告知命令调用者异常信息
    if (!string.IsNullOrEmpty(result?.ErrorReason))
    {
        await context.Channel.SendTextAsync(result.ErrorReason);
    }

    // 或者可以将结果记入日志系统
    var commandName = command.IsSpecified ? command.Value.Name : "A command";
    await _log.LogAsync(new LogMessage(LogSeverity.Info, 
        "CommandExecution", 
        $"{commandName} was executed at {DateTime.UtcNow}."));
}
public async Task HandleCommandAsync(SocketMessage msg)
{
    var message = msg as SocketUserMessage;
    if (message == null) return;
    int argPos = 0;
    if (!(message.HasCharPrefix('!', ref argPos) || 
          message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
        (message.Author.IsBot ?? false)) return;
    var context = new SocketCommandContext(_client, message);
    await _commands.ExecuteAsync(context, argPos, _services);
}