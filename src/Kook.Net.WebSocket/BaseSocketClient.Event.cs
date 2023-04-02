namespace Kook.WebSocket;

public abstract partial class BaseSocketClient
{
    #region Channels

    /// <summary> Fired when a channel is created. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a generic channel has been created. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketChannel"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The newly created channel is passed into the event handler parameter. The given channel type may
    ///         include, but not limited to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category);
    ///         see the derived classes of <see cref="SocketChannel"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketChannel, Task> ChannelCreated
    {
        add => _channelCreatedEvent.Add(value);
        remove => _channelCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelCreatedEvent = new();

    /// <summary> Fired when a channel is destroyed. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a generic channel has been destroyed. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketChannel"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The destroyed channel is passed into the event handler parameter. The given channel type may
    ///         include, but not limited to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category);
    ///         see the derived classes of <see cref="SocketChannel"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketChannel, Task> ChannelDestroyed
    {
        add => _channelDestroyedEvent.Add(value);
        remove => _channelDestroyedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelDestroyedEvent = new();

    /// <summary> Fired when a channel is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a generic channel has been updated. The event handler must return a
    ///         <see cref="Task"/> and accept 2 <see cref="SocketChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         The original (prior to update) channel is passed into the first <see cref="SocketChannel"/>, while
    ///         the updated channel is passed into the second. The given channel type may include, but not limited
    ///         to, Private Channels (DM, Group), Guild Channels (Text, Voice, Category); see the derived classes of
    ///         <see cref="SocketChannel"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated
    {
        add => _channelUpdatedEvent.Add(value);
        remove => _channelUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketChannel, SocketChannel, Task>> _channelUpdatedEvent = new();

    /// <summary> Fired when a reaction is added to a channel message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a reaction is added to a message in a channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an
    ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the <see cref="Guid"/>
    ///     </para>
    ///     <para>
    ///         The source channel of the reaction addition will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The reaction that was added will be passed into the <see cref="SocketReaction"/> parameter.
    ///     </para>
    ///     <note>
    ///         When fetching the reaction from this event, a user may not be provided under
    ///         <see cref="SocketReaction.User"/>. Please see the documentation of the property for more
    ///         information.
    ///     </note>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded
    {
        add => _reactionAddedEvent.Add(value);
        remove => _reactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, SocketReaction, Task>> _reactionAddedEvent = new();

    /// <summary> Fired when a reaction is removed from a message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a reaction is removed from a message in a channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an
    ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the <see cref="Guid"/>
    ///     </para>
    ///     <para>
    ///         The source channel of the reaction addition will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The reaction that was removed will be passed into the <see cref="SocketReaction"/> parameter.
    ///     </para>
    ///     <note>
    ///         When fetching the reaction from this event, a user may not be provided under
    ///         <see cref="SocketReaction.User"/>. Please see the documentation of the property for more
    ///         information.
    ///     </note>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved
    {
        add => _reactionRemovedEvent.Add(value);
        remove => _reactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, SocketReaction, Task>> _reactionRemovedEvent = new();

    /// <summary> Fired when a reaction is added to a direct message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a reaction is added to a user message in a private channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, a
    ///         <see cref="Cacheable{TEntity,TId}"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If a direct message was sent by the current user to this user, or the recipient had sent a message before
    ///         in current session, the <see cref="Cacheable{TEntity,TId}"/> entity will contains the direct message channel;
    ///         otherwise, the direct message channel has not been created yet, and the <see cref="Guid"/> as chat code will be preserved.
    ///     </para>
    ///     <para>
    ///         The reaction that was added will be passed into the <see cref="SocketReaction"/> parameter.
    ///     </para>
    ///     <note>
    ///         When fetching the reaction from this event, a user may not be provided under
    ///         <see cref="SocketReaction.User"/>. Please see the documentation of the property for more
    ///         information.
    ///     </note>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task> DirectReactionAdded
    {
        add => _directReactionAddedEvent.Add(value);
        remove => _directReactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionAddedEvent = new();

    /// <summary> Fired when a reaction is removed from a message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a reaction is removed from a user message in a private channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, a
    ///         <see cref="Cacheable{TEntity,TId}"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If a direct message was sent by the current user to this user, or the recipient had sent a message before
    ///         in current session, the <see cref="Cacheable{TEntity,TId}"/> entity will contains the direct message channel;
    ///         otherwise, the direct message channel has not been created yet, and the <see cref="Guid"/> as chat code will be preserved.
    ///     </para>
    ///     <para>
    ///         The reaction that was added will be passed into the <see cref="SocketReaction"/> parameter.
    ///     </para>
    ///     <note>
    ///         When fetching the reaction from this event, a user may not be provided under
    ///         <see cref="SocketReaction.User"/>. Please see the documentation of the property for more
    ///         information.
    ///     </note>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task> DirectReactionRemoved
    {
        add => _directReactionRemovedEvent.Add(value);
        remove => _directReactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionRemovedEvent = new();

    #endregion

    #region Messages

    /// <summary> Fired when a message is received. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is received. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketMessage"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The message that is sent to the client is passed into the event handler parameter as
    ///         <see cref="SocketMessage"/>. This message may be a system message (i.e.
    ///         <see cref="SocketSystemMessage"/>) or a user message (i.e. <see cref="SocketUserMessage"/>. See the
    ///         derived classes of <see cref="SocketMessage"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketMessage, Task> MessageReceived
    {
        add => _messageReceivedEvent.Add(value);
        remove => _messageReceivedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketMessage, Task>> _messageReceivedEvent = new();

    /// <summary> Fired when a message is deleted. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/> and
    ///         <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the message cannot be retrieved by Kook
    ///             after the message has been deleted.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the deleted message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The source channel of the removed message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, Task> MessageDeleted
    {
        add => _messageDeletedEvent.Add(value);
        remove => _messageDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, ISocketMessageChannel, Task>> _messageDeletedEvent = new();

    /// <summary> Fired when a message is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original message cannot be retrieved by
    ///             Kook after the message has been updated.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the updated message; otherwise, in event
    ///         that the entire message entity cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Task> MessageUpdated
    {
        add => _messageUpdatedEvent.Add(value);
        remove => _messageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Task>> _messageUpdatedEvent = new();

    /// <summary> Fired when a message is pinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is pinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original message cannot be retrieved by
    ///             Kook after the message has been updated.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the pinned message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The operator who pinned the message will be passed into the <see cref="SocketGuildUser"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Cacheable<SocketGuildUser, ulong>, Task> MessagePinned
    {
        add => _messagePinnedEvent.Add(value);
        remove => _messagePinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Cacheable<SocketGuildUser, ulong>, Task>> _messagePinnedEvent = new();

    /// <summary> Fired when a message is unpinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is unpinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original message cannot be retrieved by
    ///             Kook after the message has been updated.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the unpinned message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The operator who unpinned the message will be passed into the <see cref="SocketGuildUser"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Cacheable<SocketGuildUser, ulong>, Task> MessageUnpinned
    {
        add => _messageUnpinnedEvent.Add(value);
        remove => _messageUnpinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketMessage, Guid>, Cacheable<SocketMessage, Guid>, ISocketMessageChannel, Cacheable<SocketGuildUser, ulong>, Task>> _messageUnpinnedEvent = new();

    #endregion

    #region Direct Messages

    /// <summary> Fired when a direct message is received. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a direct message is received. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketMessage"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The message that is sent to the client is passed into the event handler parameter as
    ///         <see cref="SocketMessage"/>. This message may be a system message (i.e.
    ///         <see cref="SocketSystemMessage"/>) or a user message (i.e. <see cref="SocketUserMessage"/>. See the
    ///         derived classes of <see cref="SocketMessage"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketMessage, Task> DirectMessageReceived
    {
        add => _directMessageReceivedEvent.Add(value);
        remove => _directMessageReceivedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketMessage, Task>> _directMessageReceivedEvent = new();

    /// <summary> Fired when a direct message is deleted. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a direct message is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/> and
    ///         <see cref="IDMChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the direct message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original direct message cannot be retrieved by Kook
    ///             after the message has been deleted.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the deleted direct message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the direct message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If a direct message was sent by the current user to this user, or the recipient had sent a message before
    ///         in current session, the <see cref="Cacheable{TEntity,TId}"/> entity will contains the direct message channel;
    ///         otherwise, the direct message channel has not been created yet, and the <see cref="Guid"/> as chat code will be preserved.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, Task> DirectMessageDeleted
    {
        add => _directMessageDeletedEvent.Add(value);
        remove => _directMessageDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, Task>> _directMessageDeletedEvent = new();

    /// <summary> Fired when a message is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a direct message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/> and
    ///         <see cref="IDMChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the direct message via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original direct message cannot be retrieved by Kook
    ///             after the message has been updated.
    ///         </note>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the updated direct message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the direct message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         If a direct message was sent by the current user to this user, or the recipient had sent a message before
    ///         in current session, the <see cref="Cacheable{TEntity,TId}"/> entity will contains the direct message channel;
    ///         otherwise, the direct message channel has not been created yet, and the <see cref="Guid"/> as chat code will be preserved.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, Cacheable<SocketMessage, Guid>, IDMChannel, Task> DirectMessageUpdated
    {
        add => _directMessageUpdatedEvent.Add(value);
        remove => _directMessageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<SocketMessage, Guid>, IDMChannel, Task>> _directMessageUpdatedEvent = new();

    #endregion

    #region Users

    /// <summary> Fired when a user joins a guild. </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         It is reported that this event will not be fired if a guild contains more than 2000 members.
    ///     </note>
    ///     <para>
    ///         This event is fired when a user joins a guild. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketGuildUser"/> and a <see cref="DateTimeOffset"/>
    ///         as its parameters.
    ///     </para>
    ///     <para>
    ///         The joined user will be passed into the <see cref="SocketGuildUser"/> parameter.
    ///     </para>
    ///     <para>
    ///         The time at which the user joined the guild will be passed into the <see cref="DateTimeOffset"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuildUser, DateTimeOffset, Task> UserJoined
    {
        add => _userJoinedEvent.Add(value);
        remove => _userJoinedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuildUser, DateTimeOffset, Task>> _userJoinedEvent = new();

    /// <summary> Fired when a user leaves a guild. </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         It is reported that this event will not be fired if a guild contains more than 2000 members.
    ///     </note>
    ///     <para>
    ///         This event is fired when a user leaves a guild. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketGuildUser"/> and a <see cref="DateTimeOffset"/>
    ///         as its parameters.
    ///     </para>
    ///     <para>
    ///         If the left user presents in the cache, the <see cref="Cacheable{TEntity,TId}"/> entity
    ///         will contain the left user; otherwise, in event that the user cannot be retrieved,
    ///         the ID of the left user is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The time at which the user left the guild will be passed into the <see cref="DateTimeOffset"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, Cacheable<SocketUser, ulong>, DateTimeOffset, Task> UserLeft
    {
        add => _userLeftEvent.Add(value);
        remove => _userLeftEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Cacheable<SocketUser, ulong>, DateTimeOffset, Task>> _userLeftEvent = new();

    /// <summary> Fired when a user is banned from a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user is banned. The event handler must return a
    ///         <see cref="Task"/> and accept an <see cref="IReadOnlyCollection{T}"/>, a <see cref="SocketMessage"/>,
    ///         a <see langword="string"/> and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the user via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original user cannot be retrieved by
    ///             Kook after the user has been banned.
    ///         </note>
    ///         The users that are banned are passed into the event handler parameter as
    ///         <see cref="IReadOnlyCollection{T}"/>, where <c>T</c> is <see cref="Cacheable{TEntity,TId}"/>,
    ///         each of which contains a <see cref="SocketUser"/> when the user presents in the cache; otherwise,
    ///         in event that the user cannot be retrieved, the ID of the user is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The users who operated the bans is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the banning action takes place is passed in the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    ///     <para>
    ///         The reason of the ban is passed into the event handler parameter as <see langword="string"/>.
    ///     </para>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketUser, ulong>, SocketGuild, string, Task> UserBanned
    {
        add => _userBannedEvent.Add(value);
        remove => _userBannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketUser, ulong>, SocketGuild, string, Task>> _userBannedEvent = new();

    /// <summary> Fired when a user is unbanned from a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user is unbanned. The event handler must return a
    ///         <see cref="Task"/> and accept an <see cref="IReadOnlyCollection{T}"/>, a <see cref="SocketMessage"/>
    ///         and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the user via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original user cannot be retrieved by
    ///             Kook after the user has been unbanned.
    ///         </note>
    ///         The users that are unbanned are passed into the event handler parameter as
    ///         <see cref="IReadOnlyCollection{T}"/>, where <c>T</c> is <see cref="Cacheable{TEntity,TId}"/>,
    ///         each of which contains a <see cref="SocketUser"/> when the user presents in the cache; otherwise,
    ///         in event that the user cannot be retrieved, the ID of the user is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The users who operated the unbans is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the unbanning action takes place is passed in the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketUser, ulong>, SocketGuild, Task> UserUnbanned
    {
        add => _userUnbannedEvent.Add(value);
        remove => _userUnbannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketUser, ulong>>, Cacheable<SocketUser, ulong>, SocketGuild, Task>> _userUnbannedEvent = new();

    /// <summary> Fired when a user is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         and a <see cref="Cacheable{TEntity,TId}"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the user via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original user cannot be retrieved by
    ///             Kook after the user has been updated.
    ///         </note>
    ///         The user that is updated is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The user that is updated is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketUser, ulong>, Cacheable<SocketUser, ulong>, Task> UserUpdated
    {
        add => _userUpdatedEvent.Add(value);
        remove => _userUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketUser, ulong>, Cacheable<SocketUser, ulong>, Task>> _userUpdatedEvent = new();

    /// <summary> Fired when the connected account is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when the connected account is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketSelfUser"/>,
    ///         and a <see cref="SocketSelfUser"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The current user before the update is passed into the event handler parameter as
    ///         <see cref="SocketSelfUser"/>.
    ///     </para>
    ///     <para>
    ///         The current user after the update is passed into the event handler parameter as
    ///         <see cref="SocketSelfUser"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
    {
        add => _currentUserUpdatedEvent.Add(value);
        remove => _currentUserUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _currentUserUpdatedEvent = new();

    /// <summary> Fired when a guild member is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild member is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         and a <see cref="Cacheable{TEntity,TId}"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         <note type="important">
    ///             It is not possible to retrieve the guild member via
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the original guild member cannot be retrieved by
    ///             Kook after the guild member has been updated.
    ///         </note>
    ///         The guild member that is updated is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketGuildUser"/> when the guild member
    ///         presents in the cache; otherwise, in event that the guild member cannot be retrieved, the ID of the guild member
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The guild member that is updated is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketGuildUser"/> when the guild member
    ///         presents in the cache; otherwise, in event that the guild member cannot be retrieved, the ID of the guild member
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketGuildUser, ulong>, Cacheable<SocketGuildUser, ulong>, Task> GuildMemberUpdated
    {
        add => _guildMemberUpdatedEvent.Add(value);
        remove => _guildMemberUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, Cacheable<SocketGuildUser, ulong>, Task>> _guildMemberUpdatedEvent = new();

    /// <summary> Fired when a guild member is online. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild member is online. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         and a <see cref="DateTimeOffset"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild member that is online is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketGuildUser"/> when the guild member
    ///         presents in the cache; otherwise, in event that the guild member cannot be retrieved, the ID of the guild member
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The time when the guild member is online is passed into the event handler parameter as
    ///         <see cref="DateTimeOffset"/>.
    ///     </para>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task> GuildMemberOnline
    {
        add => _guildMemberOnlineEvent.Add(value);
        remove => _guildMemberOnlineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task>> _guildMemberOnlineEvent = new();

    /// <summary> Fired when a guild member is offline. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild member is offline. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         and a <see cref="DateTimeOffset"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild member that is offline is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketGuildUser"/> when the guild member
    ///         presents in the cache; otherwise, in event that the guild member cannot be retrieved, the ID of the guild member
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The time when the guild member is offline is passed into the event handler parameter as
    ///         <see cref="DateTimeOffset"/>.
    ///     </para>
    /// </remarks>
    public event Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task> GuildMemberOffline
    {
        add => _guildMemberOfflineEvent.Add(value);
        remove => _guildMemberOfflineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>, DateTimeOffset, Task>> _guildMemberOfflineEvent = new();

    #endregion

    #region Voices

    /// <summary> Fired when a user connected to a voice channel. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user connected to a voice channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         a <see cref="SocketVoiceChannel"/>, a <see cref="SocketGuild"/>, and a <see cref="DateTimeOffset"/>
    ///         as its parameter.
    ///     </para>
    ///     <para>
    ///         The user that connected to a voice channel is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketGuildUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The voice channel that the user connected to is passed into the event handler parameter as
    ///         <see cref="SocketVoiceChannel"/>.
    ///     </para>
    ///     <para>
    ///         The time when the user is offline is passed into the event handler parameter as
    ///         <see cref="DateTimeOffset"/>.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task> UserConnected
    {
        add => _userConnectedEvent.Add(value);
        remove => _userConnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task>> _userConnectedEvent = new();

    /// <summary> Fired when a user disconnected to a voice channel. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user disconnected to a voice channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>,
    ///         a <see cref="SocketVoiceChannel"/>, a <see cref="SocketGuild"/>, and a <see cref="DateTimeOffset"/>
    ///         as its parameter.
    ///     </para>
    ///     <para>
    ///         The user that disconnected to a voice channel is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains the original <see cref="SocketGuildUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         The voice channel that the user disconnected to is passed into the event handler parameter as
    ///         <see cref="SocketVoiceChannel"/>.
    ///     </para>
    ///     <para>
    ///         The time when the user is offline is passed into the event handler parameter as
    ///         <see cref="DateTimeOffset"/>.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<SocketUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task> UserDisconnected
    {
        add => _userDisconnectedEvent.Add(value);
        remove => _userDisconnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketUser, ulong>, SocketVoiceChannel, DateTimeOffset, Task>> _userDisconnectedEvent = new();

    #endregion

    #region Roles

    /// <summary> Fired when a role is created. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a role is created. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketRole"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The role that is created is passed into the event handler parameter as
    ///         <see cref="SocketRole"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketRole, Task> RoleCreated
    {
        add => _roleCreatedEvent.Add(value);
        remove => _roleCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new();

    /// <summary> Fired when a role is deleted. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a role is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketRole"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The role that is deleted is passed into the event handler parameter as
    ///         <see cref="SocketRole"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketRole, Task> RoleDeleted
    {
        add => _roleDeletedEvent.Add(value);
        remove => _roleDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new();

    /// <summary> Fired when a role is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a role is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketRole"/> and a <see cref="SocketRole"/>
    ///         as its parameter.
    ///     </para>
    ///     <para>
    ///         The original role entity is passed into the event handler parameter as
    ///         <see cref="SocketRole"/>.
    ///     </para>
    ///     <para>
    ///         The updated role entity is passed into the event handler parameter as
    ///         <see cref="SocketRole"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketRole, SocketRole, Task> RoleUpdated
    {
        add => _roleUpdatedEvent.Add(value);
        remove => _roleUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new();

    #endregion

    #region Emotes

    /// <summary> Fired when an emote is created. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when an emote is created. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="GuildEmote"/> and a <see cref="SocketGuild"/>
    ///         as its parameter.
    ///     </para>
    ///     <para>
    ///         The emote that is created is passed into the event handler parameter as
    ///         <see cref="GuildEmote"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the emote is created is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<GuildEmote, SocketGuild, Task> EmoteCreated
    {
        add => _emoteCreatedEvent.Add(value);
        remove => _emoteCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteCreatedEvent = new();

    /// <summary> Fired when a emote is deleted. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when an emote is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="GuildEmote"/> and a <see cref="SocketGuild"/>
    ///         as its parameter.
    ///     </para>
    ///     <para>
    ///         The emote that is deleted is passed into the event handler parameter as
    ///         <see cref="GuildEmote"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the emote is deleted is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<GuildEmote, SocketGuild, Task> EmoteDeleted
    {
        add => _emoteDeletedEvent.Add(value);
        remove => _emoteDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteDeletedEvent = new();

    /// <summary> Fired when a emote is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when an emote is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="GuildEmote"/>, a <see cref="GuildEmote"/>
    ///         and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The original emote entity is passed into the event handler parameter as
    ///         <see cref="GuildEmote"/>.
    ///     </para>
    ///     <para>
    ///         The updated emote entity is passed into the event handler parameter as
    ///         <see cref="GuildEmote"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the emote is updated is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<GuildEmote, GuildEmote, SocketGuild, Task> EmoteUpdated
    {
        add => _emoteUpdatedEvent.Add(value);
        remove => _emoteUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, GuildEmote, SocketGuild, Task>> _emoteUpdatedEvent = new();

    #endregion

    #region Guilds

    /// <summary> Fired when the connected account joins a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when the connected account joins a guild. The event handler must
    ///         return a <see cref="Task"/> and accept a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild where the account joins is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, Task> JoinedGuild
    {
        add => _joinedGuildEvent.Add(value);
        remove => _joinedGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new();

    /// <summary> Fired when the connected account leaves a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when the connected account leaves a guild. The event handler must
    ///         return a <see cref="Task"/> and accept a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild where the account leaves is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, Task> LeftGuild
    {
        add => _leftGuildEvent.Add(value);
        remove => _leftGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new();

    /// <summary> Fired when a guild is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketGuild"/>,
    ///         and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild before the update is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    ///     <para>
    ///         The guild after the update is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, SocketGuild, Task> GuildUpdated
    {
        add => _guildUpdatedEvent.Add(value);
        remove => _guildUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new();

    /// <summary> Fired when a guild becomes available. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild becomes available. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild that becomes available is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, Task> GuildAvailable
    {
        add => _guildAvailableEvent.Add(value);
        remove => _guildAvailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new();

    /// <summary> Fired when a guild becomes unavailable. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a guild becomes unavailable. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The guild that becomes unavailable is passed into the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    public event Func<SocketGuild, Task> GuildUnavailable
    {
        add => _guildUnavailableEvent.Add(value);
        remove => _guildUnavailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new();

    #endregion

    #region Interactions

    /// <summary> Fired when a button is clicked in a card message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a button is clicked in a card message. The event handler must
    ///         return a <see cref="Task"/> and accept a <see langword="string"/>,
    ///         a <see cref="Cacheable{TEntity,TId}"/>, a <see cref="Cacheable{TEntity,TId}"/>,
    ///         a <see cref="SocketTextChannel"/>, and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The button value is passed into the event handler parameter as <see langword="string"/>.
    ///     </para>
    ///     <para>
    ///         The users who clicked the button is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the card message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the <see cref="Guid"/>
    ///     </para>
    ///     <para>
    ///         The channel where the button is clicked is passed into the event handler parameter as
    ///         <see cref="SocketTextChannel"/>.
    ///     </para>
    /// </remarks>
    public event Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketTextChannel, Task> MessageButtonClicked
    {
        add => _messageButtonClickedEvent.Add(value);
        remove => _messageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketTextChannel, Task>> _messageButtonClickedEvent = new();

    /// <summary> Fired when a button is clicked in a direct card message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a button is clicked in a direct card message. The event handler must
    ///         return a <see cref="Task"/> and accept a <see langword="string"/>,
    ///         a <see cref="Cacheable{TEntity,TId}"/>, a <see cref="Cacheable{TEntity,TId}"/>,
    ///         and a <see cref="SocketTextChannel"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The button value is passed into the event handler parameter as <see langword="string"/>.
    ///     </para>
    ///     <para>
    ///         The users who clicked the button is passed into the event handler parameter as
    ///         <see cref="Cacheable{TEntity,TId}"/>, which contains a <see cref="SocketUser"/> when the user
    ///         presents in the cache; otherwise, in event that the user cannot be retrieved, the ID of the user
    ///         is preserved in the <see cref="ulong"/>.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the direct card message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the <see cref="Guid"/>
    ///     </para>
    ///     <para>
    ///         The channel where the button is clicked is passed into the event handler parameter as
    ///         <see cref="SocketTextChannel"/>.
    ///     </para>
    /// </remarks>
    public event Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketDMChannel, Task> DirectMessageButtonClicked
    {
        add => _directMessageButtonClickedEvent.Add(value);
        remove => _directMessageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, Cacheable<SocketUser, ulong>, Cacheable<IMessage, Guid>, SocketDMChannel, Task>> _directMessageButtonClickedEvent = new();

    #endregion
}
