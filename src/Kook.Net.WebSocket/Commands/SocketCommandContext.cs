using Kook.WebSocket;

namespace Kook.Commands;

/// <summary>
///     表示一个基于 WebSocket 客户端的命令的上下文。这可能包括客户端、公会、频道、用户和消息。
/// </summary>
public class SocketCommandContext : ICommandContext
{
    #region SocketCommandContext

    /// <summary>
    ///     获取命令执行时所使用的 <see cref="T:Kook.WebSocket.KookSocketClient" />。
    /// </summary>
    public KookSocketClient Client { get; }

    /// <summary>
    ///     获取命令执行所在的 <see cref="T:Kook.WebSocket.SocketGuild" />。
    /// </summary>
    public SocketGuild? Guild { get; }

    /// <summary>
    ///     获取命令执行所在的 <see cref="T:Kook.WebSocket.ISocketMessageChannel" />。
    /// </summary>
    public ISocketMessageChannel Channel { get; }

    /// <summary>
    ///     获取执行命令的 <see cref="T:Kook.WebSocket.SocketUser" />。
    /// </summary>
    public SocketUser User { get; }

    /// <summary>
    ///     获取命令解析的源 <see cref="T:Kook.WebSocket.SocketUserMessage" />。
    /// </summary>
    public SocketUserMessage Message { get; }

    /// <summary>
    ///     指示命令执行所在的频道是否为 <see cref="T:Kook.IPrivateChannel"/> 私聊频道。
    /// </summary>
    public bool IsPrivate => Channel is IPrivateChannel;

    /// <summary>
    ///     使用提供的客户端和消息初始化一个 <see cref="SocketCommandContext" /> 类的新实例。
    /// </summary>
    /// <param name="client"> 底层客户端。 </param>
    /// <param name="msg"> 底层消息。 </param>
    public SocketCommandContext(KookSocketClient client, SocketUserMessage msg)
    {
        Client = client;
        Guild = (msg.Channel as SocketGuildChannel)?.Guild;
        Channel = msg.Channel;
        User = msg.Author;
        Message = msg;
    }

    #endregion

    #region ICommandContext

    /// <inheritdoc/>
    IKookClient ICommandContext.Client => Client;

    /// <inheritdoc/>
    IGuild? ICommandContext.Guild => Guild;

    /// <inheritdoc/>
    IMessageChannel ICommandContext.Channel => Channel;

    /// <inheritdoc/>
    IUser ICommandContext.User => User;

    /// <inheritdoc/>
    IUserMessage ICommandContext.Message => Message;

    #endregion
}
