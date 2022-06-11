public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
{
    switch(result)
    {
        case MyCustomResult customResult:
            // 提取自定义结果类中的信息进行进一步的处理
            break;
        default:
            if (!string.IsNullOrEmpty(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);
            break;
    }
}