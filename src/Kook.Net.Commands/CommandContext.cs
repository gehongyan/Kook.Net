namespace Kook.Commands;

/// <inheritdoc />
public class CommandContext : ICommandContext
{
    /// <inheritdoc/>
    public IKookClient Client { get; }

    /// <inheritdoc/>
    public IGuild? Guild { get; }

    /// <inheritdoc/>
    public IMessageChannel Channel { get; }

    /// <inheritdoc/>
    public IUser User { get; }

    /// <inheritdoc/>
    public IUserMessage Message { get; }

    /// <summary>
    ///     获取当前上下文是否为私有的执行上下文。
    /// </summary>
    public bool IsPrivate => Channel is IPrivateChannel;

    /// <summary>
    ///     初始化一个包含指定客户端和消息的 <see cref="CommandContext" /> 类的新实例。
    /// </summary>
    /// <param name="client"> 命令执行时所使用的客户端。 </param>
    /// <param name="msg"> 命令解析的源消息。 </param>
    public CommandContext(IKookClient client, IUserMessage msg)
    {
        Client = client;
        Guild = (msg.Channel as IGuildChannel)?.Guild;
        Channel = msg.Channel;
        User = msg.Author;
        Message = msg;
    }
}
