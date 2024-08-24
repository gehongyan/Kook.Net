namespace Kook.WebSocket;

public abstract partial class BaseSocketClient
{
    #region Channels

    /// <summary>
    ///     当服务器频道被创建时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是新创建的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketChannel, Task> ChannelCreated
    {
        add => _channelCreatedEvent.Add(value);
        remove => _channelCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelCreatedEvent = new();

    /// <summary>
    ///     当服务器频道被删除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是被删除的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketChannel, Task> ChannelDestroyed
    {
        add => _channelDestroyedEvent.Add(value);
        remove => _channelDestroyedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelDestroyedEvent = new();

    /// <summary>
    ///     当服务器频道信息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是更新前的服务器频道。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是更新后的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated
    {
        add => _channelUpdatedEvent.Add(value);
        remove => _channelUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, SocketChannel, Task>> _channelUpdatedEvent = new();

    /// <summary>
    ///     当服务器内的消息上被添加了新的回应时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被添加了回应的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是消息所在的频道。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是添加了此回应的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketReaction"/> 参数是被添加的回应。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, SocketReaction, Task> ReactionAdded
    {
        add => _reactionAddedEvent.Add(value);
        remove => _reactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, SocketReaction, Task>> _reactionAddedEvent = new();

    /// <summary>
    ///     当服务器内的消息上存在的回应被用户移除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被移除了回应的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketChannel"/> 参数是消息所在的频道。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是移除了此回应的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketReaction"/> 参数是被移除的回应。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, SocketReaction, Task> ReactionRemoved
    {
        add => _reactionRemovedEvent.Add(value);
        remove => _reactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, SocketReaction, Task>> _reactionRemovedEvent = new();

    /// <summary>
    ///     当私聊频道内的消息上被添加了新的回应时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被添加了回应的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是消息所在的可缓存私聊频道。如果缓存中存在此私聊频道实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketDMChannel"/> 私聊频道；否则，包含 <see cref="T:System.Guid"/> 私聊频道聊天代码，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是添加了此回应的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketReaction"/> 参数是被添加的回应。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<SocketDMChannel, Guid>, Cacheable<SocketUser, ulong>, SocketReaction, Task> DirectReactionAdded
    {
        add => _directReactionAddedEvent.Add(value);
        remove => _directReactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<SocketDMChannel, Guid>, Cacheable<SocketUser, ulong>, SocketReaction, Task>> _directReactionAddedEvent = new();

    /// <summary>
    ///     当私聊频道内的消息上存在的回应被用户移除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被移除了回应的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是消息所在的可缓存私聊频道。如果缓存中存在此私聊频道实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketDMChannel"/> 私聊频道；否则，包含 <see cref="T:System.Guid"/> 私聊频道聊天代码，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是移除了此回应的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketReaction"/> 参数是被移除的回应。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<SocketDMChannel, Guid>, Cacheable<SocketUser, ulong>, SocketReaction, Task> DirectReactionRemoved
    {
        add => _directReactionRemovedEvent.Add(value);
        remove => _directReactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<SocketDMChannel, Guid>, Cacheable<SocketUser, ulong>, SocketReaction, Task>> _directReactionRemovedEvent = new();

    #endregion

    #region Messages

    /// <summary>
    ///     当接收到新的服务器消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketMessage"/> 参数是新接收到的服务器消息。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuildUser"/> 参数是发送消息的服务器用户。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息所在的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketMessage, SocketGuildUser, SocketTextChannel, Task> MessageReceived
    {
        add => _messageReceivedEvent.Add(value);
        remove => _messageReceivedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketMessage, SocketGuildUser, SocketTextChannel, Task>> _messageReceivedEvent = new();

    /// <summary>
    ///     当服务器内的消息被删除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被删除的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 被删除的消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被删除后无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息被删除前所在的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketTextChannel, Task> MessageDeleted
    {
        add => _messageDeletedEvent.Add(value);
        remove => _messageDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketTextChannel, Task>> _messageDeletedEvent = new();

    /// <summary>
    ///     当服务器内的消息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被更新前的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新前的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被更新前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被更新后的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息所在的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Task> MessageUpdated
    {
        add => _messageUpdatedEvent.Add(value);
        remove => _messageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Task>> _messageUpdatedEvent = new();

    /// <summary>
    ///     当服务器内的消息被置顶时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被置顶前的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被置顶前的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被置顶前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被置顶后的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被置顶后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息所在的服务器频道。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是置顶了该消息的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, Task> MessagePinned
    {
        add => _messagePinnedEvent.Add(value);
        remove => _messagePinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, Task>> _messagePinnedEvent = new();

    /// <summary>
    ///     当服务器内的消息被取消置顶时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被取消置顶前的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被取消置顶前的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被取消置顶前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被取消置顶后的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被取消置顶后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息所在的服务器频道。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是取消置顶了该消息的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, Task> MessageUnpinned
    {
        add => _messageUnpinnedEvent.Add(value);
        remove => _messageUnpinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, SocketTextChannel, Cacheable<SocketGuildUser, ulong>, Task>> _messageUnpinnedEvent = new();

    #endregion

    #region Direct Messages

    /// <summary>
    ///     当接收到新的私聊消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketMessage"/> 参数是新接收到的私聊消息。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketUser"/> 参数是发送消息的用户。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketDMChannel"/> 参数是消息所在的私聊频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketMessage, SocketUser, SocketDMChannel, Task> DirectMessageReceived
    {
        add => _directMessageReceivedEvent.Add(value);
        remove => _directMessageReceivedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketMessage, SocketUser, SocketDMChannel, Task>> _directMessageReceivedEvent = new();

    /// <summary>
    ///     当私聊频道内的消息被删除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是被删除的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 被删除的消息；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被删除后无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是该消息的作者的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是消息被删除前所在的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<SocketUser, ulong>, Cacheable<SocketDMChannel, Guid>, Task> DirectMessageDeleted
    {
        add => _directMessageDeletedEvent.Add(value);
        remove => _directMessageDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<SocketUser, ulong>, Cacheable<SocketDMChannel, Guid>, Task>> _directMessageDeletedEvent = new();

    /// <summary>
    ///     当私聊频道内的消息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被更新前的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新前的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID。
    ///         <br />
    ///         <note type="important">
    ///             消息被更新前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存消息被更新后的状态。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是该消息的作者的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketDMChannel"/> 参数是消息所在的私聊频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, Cacheable<SocketUser, ulong>, Cacheable<SocketDMChannel, Guid>, Task> DirectMessageUpdated
    {
        add => _directMessageUpdatedEvent.Add(value);
        remove => _directMessageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessage, Guid>, Cacheable<SocketUser, ulong>, Cacheable<SocketDMChannel, Guid>, Task>> _directMessageUpdatedEvent = new();

    #endregion

    #region Users

    /// <summary>
    ///     当用户加入服务器时引发。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         有消息称，那么此事件不会在其成员数量超过 2000 人的服务器内被触发。
    ///     </note>
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是加入服务器的可缓存服务器用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是用户加入服务器的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, DateTimeOffset, Task> UserJoined
    {
        add => _userJoinedEvent.Add(value);
        remove => _userJoinedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, DateTimeOffset, Task>> _userJoinedEvent = new();

    /// <summary>
    ///     当用户离开服务器时引发。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         有消息称，那么此事件不会在其成员数量超过 2000 人的服务器内被触发。
    ///     </note>
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是用户离开的服务器。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是离开服务器的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是用户加入服务器的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, Cacheable<SocketUser, ulong>, DateTimeOffset, Task> UserLeft
    {
        add => _userLeftEvent.Add(value);
        remove => _userLeftEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Cacheable<SocketUser, ulong>, DateTimeOffset, Task>> _userLeftEvent = new();

    /// <summary>
    ///     当用户被服务器封禁时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:System.Collections.Generic.IReadOnlyCollection`1"/> 参数是所有此批次被封禁的用户。每个用户都由一个
    ///         <see cref="T:Kook.Cacheable`2"/> 表示。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是操作此批次封禁的可缓存服务器用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是封禁操作所在的服务器。 </item>
    ///     <item> <see cref="T:System.String"/> 参数是封禁的原因。 </item>
    ///     </list>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketGuildUser, ulong>, SocketGuild, string?, Task> UserBanned
    {
        add => _userBannedEvent.Add(value);
        remove => _userBannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketGuildUser, ulong>, SocketGuild, string?, Task>> _userBannedEvent = new();

    /// <summary>
    ///     当用户在服务器内的封禁被解除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:System.Collections.Generic.IReadOnlyCollection`1"/> 参数是所有此批次被解除封禁的用户。每个用户都由一个
    ///         <see cref="T:Kook.Cacheable`2"/> 表示。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是操作此批次解除封禁的可缓存服务器用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是解除封禁操作所在的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketGuildUser, ulong>, SocketGuild, Task> UserUnbanned
    {
        add => _userUnbannedEvent.Add(value);
        remove => _userUnbannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketGuildUser, ulong>, SocketGuild, Task>> _userUnbannedEvent = new();

    /// <summary>
    ///     当用户信息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     当用户的用户名或头像变更，且该用户与当前用户存在聊天会话或互为好友时，此事件会被引发。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存用户被更新前的状态。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketUser"/> 用户被更新前的状态；否则，包含 <see cref="T:System.UInt64"/> 用户 ID。
    ///         <br />
    ///         <note type="important">
    ///             用户被更新前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存用户被更新后的状态。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketUser"/> 用户被更新后的状态；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketUser, ulong>, Cacheable<SocketUser, ulong>, Task> UserUpdated
    {
        add => _userUpdatedEvent.Add(value);
        remove => _userUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketUser, ulong>, Cacheable<SocketUser, ulong>, Task>> _userUpdatedEvent = new();

    /// <summary>
    ///     当当前用户信息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketSelfUser"/> 参数是当前用户被更新前的状态。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketSelfUser"/> 参数是当前用户被更新后的状态。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
    {
        add => _currentUserUpdatedEvent.Add(value);
        remove => _currentUserUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _currentUserUpdatedEvent = new();

    /// <summary>
    ///     当服务器用户信息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 参数是可缓存服务器用户被更新前的状态。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户被更新前的状态；否则，包含 <see cref="T:System.UInt64"/> 用户 ID。
    ///         <br />
    ///         <note type="important">
    ///             服务器用户被更新前的状态无法通过 <see cref="M:Kook.Cacheable`2.DownloadAsync"/> 方法下载。
    ///         </note>
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是可缓存服务器用户被更新后的状态。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketGuildUser"/> 用户被更新后的状态；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, Cacheable<SocketGuildUser, ulong>, Task> GuildMemberUpdated
    {
        add => _guildMemberUpdatedEvent.Add(value);
        remove => _guildMemberUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, Cacheable<SocketGuildUser, ulong>, Task>> _guildMemberUpdatedEvent = new();

    /// <summary>
    ///     当服务器用户的在线状态变更为在线时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:System.Collections.Generic.IReadOnlyCollection`1"/> 参数是所有此批次在线状态变更为在线的服务器用户。每个服务器用户都由一个
    ///         <see cref="T:Kook.Cacheable`2"/> 表示。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是在线状态变更为在线的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task> GuildMemberOnline
    {
        add => _guildMemberOnlineEvent.Add(value);
        remove => _guildMemberOnlineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task>> _guildMemberOnlineEvent = new();

    /// <summary>
    ///     当服务器用户的在线状态变更为离线时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:System.Collections.Generic.IReadOnlyCollection`1"/> 参数是所有此批次在线状态变更为离线的服务器用户。每个服务器用户都由一个
    ///         <see cref="T:Kook.Cacheable`2"/> 表示。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.WebSocket.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是在线状态变更为离线的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task> GuildMemberOffline
    {
        add => _guildMemberOfflineEvent.Add(value);
        remove => _guildMemberOfflineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task>> _guildMemberOfflineEvent = new();

    /// <summary>
    ///     当服务器用户的语音状态发生变化时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是其语音状态发生变化的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceState"/> 参数是用户语音状态变更前的状态。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceState"/> 参数是用户语音状态变更后的状态。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated
    {
        add => _userVoiceStateUpdatedEvent.Add(value);
        remove => _userVoiceStateUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceState, SocketVoiceState, Task>> _userVoiceStateUpdatedEvent = new();

    #endregion

    #region Voices

    /// <summary>
    ///     当服务器用户连接到语音频道时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是连接到语音频道的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceChannel"/> 参数是用户连接到的语音频道。 </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是用户连接到语音频道的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task> UserConnected
    {
        add => _userConnectedEvent.Add(value);
        remove => _userConnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task>> _userConnectedEvent = new();

    /// <summary>
    ///     当服务器用户从语音频道断开连接时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是从语音频道断开连接的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceChannel"/> 参数是用户断开连接的语音频道。 </item>
    ///     <item> <see cref="T:System.DateTimeOffset"/> 参数是用户断开连接的时间。 </item>
    ///     </list>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task> UserDisconnected
    {
        add => _userDisconnectedEvent.Add(value);
        remove => _userDisconnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task>> _userDisconnectedEvent = new();

    // /// <summary>
    // ///     当服务器用户开始直播时引发。
    // /// </summary>
    // /// <remarks>
    // ///     事件参数：
    // ///     <list type="number">
    // ///     <item>
    // ///         <see cref="T:Kook.Cacheable`2"/> 参数是开始直播的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    // ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    // ///     </item>
    // ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceChannel"/> 参数是用户开始直播的语音频道。 </item>
    // ///     </list>
    // /// </remarks>
    // public event Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, Task> LivestreamBegan
    // {
    //     add => _livestreamBeganEvent.Add(value);
    //     remove => _livestreamBeganEvent.Remove(value);
    // }
    //
    // internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, Task>> _livestreamBeganEvent = new();
    //
    // /// <summary>
    // ///     当服务器用户结束直播时引发。
    // /// </summary>
    // /// <remarks>
    // ///     事件参数：
    // ///     <list type="number">
    // ///     <item>
    // ///         <see cref="T:Kook.Cacheable`2"/> 参数是结束直播的可缓存服务器用户。如果缓存中存在此服务器用户实体，那么该结构内包含该
    // ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    // ///     </item>
    // ///     <item> <see cref="T:Kook.WebSocket.SocketVoiceChannel"/> 参数是用户结束直播的语音频道。 </item>
    // ///     </list>
    // /// </remarks>
    // public event Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, Task> LivestreamEnded
    // {
    //     add => _livestreamEndedEvent.Add(value);
    //     remove => _livestreamEndedEvent.Remove(value);
    // }
    //
    // internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketVoiceChannel, Task>> _livestreamEndedEvent = new();

    #endregion

    #region Roles

    /// <summary>
    ///     当服务器内的角色被创建时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketRole"/> 参数是被创建的角色。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketRole, Task> RoleCreated
    {
        add => _roleCreatedEvent.Add(value);
        remove => _roleCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new();

    /// <summary>
    ///     当服务器内的角色被删除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketRole"/> 参数是被删除的角色。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketRole, Task> RoleDeleted
    {
        add => _roleDeletedEvent.Add(value);
        remove => _roleDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new();

    /// <summary>
    ///     当服务器内的角色被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketRole"/> 参数是角色被更新前的状态。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketRole"/> 参数是角色被更新后的状态。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketRole, SocketRole, Task> RoleUpdated
    {
        add => _roleUpdatedEvent.Add(value);
        remove => _roleUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new();

    #endregion

    #region Emotes

    /// <summary>
    ///     当服务器内的自定义表情被创建时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.GuildEmote"/> 参数是被创建的自定义表情。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是自定义表情所在的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<GuildEmote, SocketGuild, Task> EmoteCreated
    {
        add => _emoteCreatedEvent.Add(value);
        remove => _emoteCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteCreatedEvent = new();

    /// <summary>
    ///     当服务器内的自定义表情被删除时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.GuildEmote"/> 参数是被删除的自定义表情。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是自定义表情被删除前所在的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<GuildEmote, SocketGuild, Task> EmoteDeleted
    {
        add => _emoteDeletedEvent.Add(value);
        remove => _emoteDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteDeletedEvent = new();

    /// <summary>
    ///     当服务器内的自定义表情被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.GuildEmote"/> 参数是自定义表情被更新前的状态，如果缓存中不存在此自定义表情实体，则为 <see langword="null"/>。 </item>
    ///     <item> <see cref="T:Kook.GuildEmote"/> 参数是自定义表情被更新后的状态。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是自定义表情所在的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<GuildEmote?, GuildEmote, SocketGuild, Task> EmoteUpdated
    {
        add => _emoteUpdatedEvent.Add(value);
        remove => _emoteUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote?, GuildEmote, SocketGuild, Task>> _emoteUpdatedEvent = new();

    #endregion

    #region Guilds

    /// <summary>
    ///     当当前用户新加入服务器时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是当前用户新加入的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, Task> JoinedGuild
    {
        add => _joinedGuildEvent.Add(value);
        remove => _joinedGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new();

    /// <summary>
    ///     当当前用户离开服务器时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是当前用户离开的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, Task> LeftGuild
    {
        add => _leftGuildEvent.Add(value);
        remove => _leftGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new();

    /// <summary>
    ///     当服务器信息被更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是服务器信息被更新前的状态。 </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是服务器信息被更新后的状态。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, SocketGuild, Task> GuildUpdated
    {
        add => _guildUpdatedEvent.Add(value);
        remove => _guildUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new();

    /// <summary>
    ///     当服务器状态变更为可用时引发。
    /// </summary>
    /// <remarks>
    ///     服务器状态变更为可用，表示此服务器实体已完整缓存基础数据，并与网关同步。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是服务器状态变更为可用的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, Task> GuildAvailable
    {
        add => _guildAvailableEvent.Add(value);
        remove => _guildAvailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new();

    /// <summary>
    ///     当服务器状态变更为不可用时引发。
    /// </summary>
    /// <remarks>
    ///     服务器状态变更为不可用，表示此服务器实体丢失与网关的同步，所缓存的数据不可靠，这通常发生在服务器被删除、当前用户离开服务器、网关连接断开等情况。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.WebSocket.SocketGuild"/> 参数是服务器状态变更为不可用的服务器。 </item>
    ///     </list>
    /// </remarks>
    public event Func<SocketGuild, Task> GuildUnavailable
    {
        add => _guildUnavailableEvent.Add(value);
        remove => _guildUnavailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new();

    #endregion

    #region Interactions

    /// <summary>
    ///     当用户点击服务器频道内的卡片消息按钮时引发。
    /// </summary>
    /// <remarks>
    ///     卡片消息中的按钮包含多种点击事件类型，当该类型为 <see cref="F:Kook.ButtonClickEventType.ReturnValue"/>
    ///     时，用户点击按钮后会引发此事件。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.String"/> 参数是按钮的值。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是点击按钮的可缓存服务器用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketGuildUser"/> 服务器用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是点击按钮所在的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketTextChannel"/> 参数是点击按钮所在的服务器频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<string, Cacheable<SocketGuildUser, ulong>, Cacheable<IMessage, Guid>, SocketTextChannel, Task> MessageButtonClicked
    {
        add => _messageButtonClickedEvent.Add(value);
        remove => _messageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, Cacheable<SocketGuildUser, ulong>, Cacheable<IMessage, Guid>, SocketTextChannel, Task>> _messageButtonClickedEvent = new();

    /// <summary>
    ///     当用户点击私聊频道内的卡片消息按钮时引发。
    /// </summary>
    /// <remarks>
    ///     卡片消息中的按钮包含多种点击事件类型，当该类型为 <see cref="F:Kook.ButtonClickEventType.ReturnValue"/>
    ///     时，用户点击按钮后会引发此事件。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.String"/> 参数是按钮的值。 </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是点击按钮的可缓存用户。如果缓存中存在此用户实体，那么该结构内包含该
    ///         <see cref="T:Kook.SocketUser"/> 用户；否则，包含 <see cref="T:System.UInt64"/> 用户 ID，以供按需下载实体。
    ///     </item>
    ///     <item>
    ///         <see cref="T:Kook.Cacheable`2"/> 参数是点击按钮所在的可缓存消息。如果缓存中存在此消息实体，那么该结构内包含该
    ///         <see cref="T:Kook.IMessage"/> 消息被更新后的状态；否则，包含 <see cref="T:System.Guid"/> 消息 ID，以供按需下载实体。
    ///     </item>
    ///     <item> <see cref="T:Kook.WebSocket.SocketDMChannel"/> 参数是点击按钮所在的私聊频道。 </item>
    ///     </list>
    /// </remarks>
    public event Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketDMChannel, Task> DirectMessageButtonClicked
    {
        add => _directMessageButtonClickedEvent.Add(value);
        remove => _directMessageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketDMChannel, Task>> _directMessageButtonClickedEvent = new();

    #endregion
}
