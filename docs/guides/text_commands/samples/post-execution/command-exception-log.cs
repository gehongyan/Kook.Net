public async Task LogAsync(LogMessage logMessage)
{
    if (logMessage.Exception is CommandException cmdException)
    {
        // 可以告知命令调用者所发生的异常
        await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");

        // 也可以将此异常记入日志系统
        Console.WriteLine($"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
        Console.WriteLine(cmdException.ToString());
    }
}