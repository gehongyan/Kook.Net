// 此部分代码处理命令直接结果的方式不受推荐
// 请不要盲目赋值本示例的代码
IResult result = await _commands.ExecuteAsync(context, argPos, _services);
if (result.Error != null)
    switch (result.Error)
    {
        case CommandError.BadArgCount:
            await context.Channel.SendKMarkdownMessageAsync(
                "Parameter count does not match any command's.");
            break;
        default:
            await context.Channel.SendKMarkdownMessageAsync(
                $"An error has occurred {result.ErrorReason}");
            break;
    }