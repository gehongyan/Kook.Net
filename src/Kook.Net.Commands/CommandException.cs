namespace Kook.Commands;

/// <summary>
///     表示一个命令执行过程中发生的异常。
/// </summary>
public class CommandException : Exception
{
    /// <summary>
    ///     获取异常的命令信息。
    /// </summary>
    public CommandInfo Command { get; }

    /// <summary>
    ///     获取异常的命令上下文。
    /// </summary>
    public ICommandContext Context { get; }

    /// <summary>
    ///     初始化一个 <see cref="CommandException" /> 类的新实例。
    /// </summary>
    /// <param name="command"> 引发异常的命令。 </param>
    /// <param name="context"> 引发异常的命令上下文。 </param>
    /// <param name="ex"> 引发的异常。 </param>
    public CommandException(CommandInfo command, ICommandContext context, Exception? ex)
        : base($"Error occurred executing {command.GetLogText(context)}.", ex)
    {
        Command = command;
        Context = context;
    }
}
