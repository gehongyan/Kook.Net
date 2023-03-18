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
    ///         This event is fired when a reaction is added to a user message in a channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an
    ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         ulong.
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
    public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> ReactionAdded
    {
        add => _reactionAddedEvent.Add(value);
        remove => _reactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionAddedEvent =
        new();

    /// <summary> Fired when a reaction is removed from a message. </summary>
    public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> ReactionRemoved
    {
        add => _reactionRemovedEvent.Add(value);
        remove => _reactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionRemovedEvent =
        new();

    /// <summary> Fired when a reaction is added to a direct message. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a reaction is added to a user message in a private channel. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an
    ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The source channel of the reaction addition will be passed into the
    ///         <see cref="IDMChannel"/> parameter.
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
    public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task> DirectReactionAdded
    {
        add => _directReactionAddedEvent.Add(value);
        remove => _directReactionAddedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionAddedEvent =
        new();

    /// <summary> Fired when a reaction is removed from a message. </summary>
    public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task> DirectReactionRemoved
    {
        add => _directReactionRemovedEvent.Add(value);
        remove => _directReactionRemovedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionRemovedEvent =
        new();

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
    public event Func<Cacheable<IMessage, Guid>, Cacheable<IMessageChannel, ulong>, Task> MessageDeleted
    {
        add => _messageDeletedEvent.Add(value);
        remove => _messageDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessageChannel, ulong>, Task>> _messageDeletedEvent = new();

    /// <summary> Fired when a message is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The updated message will be passed into the <see cref="SocketMessage"/> parameter.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated
    {
        add => _messageUpdatedEvent.Add(value);
        remove => _messageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, Task>> _messageUpdatedEvent = new();

    /// <summary> Fired when a message is pinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is pinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The updated message will be passed into the <see cref="SocketMessage"/> parameter.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The operator who pinned the message will be passed into the <see cref="SocketGuildUser"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task> MessagePinned
    {
        add => _messagePinnedEvent.Add(value);
        remove => _messagePinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>> _messagePinnedEvent =
        new();

    /// <summary> Fired when a message is unpinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is unpinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The updated message will be passed into the <see cref="SocketMessage"/> parameter.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="ISocketMessageChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         The operator who unpinned the message will be passed into the <see cref="SocketGuildUser"/> parameter.
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task> MessageUnpinned
    {
        add => _messageUnpinnedEvent.Add(value);
        remove => _messageUnpinnedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>> _messageUnpinnedEvent =
        new();

    #endregion

    #region Direct Messages

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
    public event Func<SocketMessage, Task> DirectMessageReceived
    {
        add => _directMessageReceivedEvent.Add(value);
        remove => _directMessageReceivedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketMessage, Task>> _directMessageReceivedEvent = new();

    /// <summary> Fired when a message is deleted. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is deleted. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/> and
    ///         <see cref="IDMChannel"/> as its parameters.
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
    ///         <see cref="IDMChannel"/> parameter.
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
    ///         This event is fired when a message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="IDMChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KookSocketConfig"/>, the
    ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
    ///         that the message cannot be retrieved, the ID of the message is preserved in the
    ///         <see cref="Guid"/>.
    ///     </para>
    ///     <para>
    ///         The updated message will be passed into the <see cref="SocketMessage"/> parameter.
    ///     </para>
    ///     <para>
    ///         The source channel of the updated message will be passed into the
    ///         <see cref="IDMChannel"/> parameter.
    ///     </para>
    ///     <para>
    ///         <note type="warning">
    ///             Due to the lack of REST API for direct message info getting, this event may fire but
    ///             the message entity will be null.
    ///         </note>
    ///     </para>
    /// </remarks>
    public event Func<Cacheable<IMessage, Guid>, SocketMessage, IDMChannel, Task> DirectMessageUpdated
    {
        add => _directMessageUpdatedEvent.Add(value);
        remove => _directMessageUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, IDMChannel, Task>> _directMessageUpdatedEvent = new();

    #endregion

    #region Users

    /// <summary> Fired when a user joins a guild. </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         It is reported that this event will not be fired if a guild contains more than 2000 members.
    ///     </note>
    /// </remarks>
    public event Func<SocketGuildUser, DateTimeOffset, Task> UserJoined
    {
        add => _userJoinedEvent.Add(value);
        remove => _userJoinedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuildUser, DateTimeOffset, Task>> _userJoinedEvent = new();

    /// <summary> Fired when a user leaves a guild. </summary>
    public event Func<SocketGuild, SocketUser, DateTimeOffset, Task> UserLeft
    {
        add => _userLeftEvent.Add(value);
        remove => _userLeftEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, SocketUser, DateTimeOffset, Task>> _userLeftEvent = new();

    /// <summary> Fired when a user is banned from a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user is banned. The event handler must return a
    ///         <see cref="Task"/> and accept an <see cref="IReadOnlyCollection{T}"/>, a <see cref="SocketMessage"/>
    ///         and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The users that are banned are passed into the event handler parameter as
    ///         <see cref="IReadOnlyCollection{T}"/>.
    ///     </para>
    ///     <para>
    ///         The user who operated the bans is passed into the event handler parameter as
    ///         <see cref="SocketUser"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the banning action takes place is passed in the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    ///     <para>
    ///         The banning actions are usually taken with kicking, and the kicking action takes place
    ///         before the banning action according to the KOOK gateway events. Therefore, the banned users
    ///         parameter is usually a collection of <see cref="SocketUnknownUser"/>.
    ///     </para>
    /// </remarks>
    public event Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task> UserBanned
    {
        add => _userBannedEvent.Add(value);
        remove => _userBannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task>> _userBannedEvent = new();

    /// <summary> Fired when a user is unbanned from a guild. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a user is banned. The event handler must return a
    ///         <see cref="Task"/> and accept an <see cref="IReadOnlyCollection{T}"/>, a <see cref="SocketMessage"/>
    ///         and a <see cref="SocketGuild"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The users that are unbanned are passed into the event handler parameter as
    ///         <see cref="IReadOnlyCollection{T}"/>.
    ///     </para>
    ///     <para>
    ///         The user who operated the bans is passed into the event handler parameter as
    ///         <see cref="SocketUser"/>.
    ///     </para>
    ///     <para>
    ///         The guild where the unbanning action takes place is passed in the event handler parameter as
    ///         <see cref="SocketGuild"/>.
    ///     </para>
    /// </remarks>
    ///     <para>
    ///         The unbanning actions are usually taken to users that are not in the guild. Therefore, the unbanned users
    ///         parameter is usually a collection of <see cref="SocketUnknownUser"/>.
    ///     </para>
    public event Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task> UserUnbanned
    {
        add => _userUnbannedEvent.Add(value);
        remove => _userUnbannedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task>> _userUnbannedEvent = new();

    /// <summary> Fired when a user is updated. </summary>
    public event Func<SocketUser, SocketUser, Task> UserUpdated
    {
        add => _userUpdatedEvent.Add(value);
        remove => _userUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketUser, SocketUser, Task>> _userUpdatedEvent = new();

    /// <summary> Fired when the connected account is updated. </summary>
    public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
    {
        add => _selfUpdatedEvent.Add(value);
        remove => _selfUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _selfUpdatedEvent = new();

    /// <summary> Fired when a guild member is updated. </summary>
    public event Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task> GuildMemberUpdated
    {
        add => _guildMemberUpdatedEvent.Add(value);
        remove => _guildMemberUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>> _guildMemberUpdatedEvent = new();

    /// <summary> Fired when a guild member is online. </summary>
    public event Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task> GuildMemberOnline
    {
        add => _guildMemberOnlineEvent.Add(value);
        remove => _guildMemberOnlineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>> _guildMemberOnlineEvent = new();

    /// <summary> Fired when a guild member is offline. </summary>
    public event Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task> GuildMemberOffline
    {
        add => _guildMemberOfflineEvent.Add(value);
        remove => _guildMemberOfflineEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>> _guildMemberOfflineEvent = new();

    #endregion

    #region Voices

    /// <summary> Fired when a user connected to a voice channel. </summary>
    public event Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task> UserConnected
    {
        add => _userConnectedEvent.Add(value);
        remove => _userConnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>> _userConnectedEvent = new();

    /// <summary> Fired when a user disconnected to a voice channel. </summary>
    public event Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task> UserDisconnected
    {
        add => _userDisconnectedEvent.Add(value);
        remove => _userDisconnectedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>> _userDisconnectedEvent = new();

    #endregion

    #region Roles

    /// <summary> Fired when a role is created. </summary>
    public event Func<SocketRole, Task> RoleCreated
    {
        add => _roleCreatedEvent.Add(value);
        remove => _roleCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new();

    /// <summary> Fired when a role is deleted. </summary>
    public event Func<SocketRole, Task> RoleDeleted
    {
        add => _roleDeletedEvent.Add(value);
        remove => _roleDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new();

    /// <summary> Fired when a role is updated. </summary>
    public event Func<SocketRole, SocketRole, Task> RoleUpdated
    {
        add => _roleUpdatedEvent.Add(value);
        remove => _roleUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new();

    #endregion

    #region Emotes

    /// <summary> Fired when a emote is created. </summary>
    public event Func<GuildEmote, SocketGuild, Task> EmoteCreated
    {
        add => _emoteCreatedEvent.Add(value);
        remove => _emoteCreatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteCreatedEvent = new();

    /// <summary> Fired when a emote is deleted. </summary>
    public event Func<GuildEmote, SocketGuild, Task> EmoteDeleted
    {
        add => _emoteDeletedEvent.Add(value);
        remove => _emoteDeletedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, SocketGuild, Task>> _emoteDeletedEvent = new();

    /// <summary> Fired when a emote is updated. </summary>
    public event Func<GuildEmote, GuildEmote, SocketGuild, Task> EmoteUpdated
    {
        add => _emoteUpdatedEvent.Add(value);
        remove => _emoteUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<GuildEmote, GuildEmote, SocketGuild, Task>> _emoteUpdatedEvent = new();

    #endregion

    #region Guilds

    /// <summary> Fired when the connected account joins a guild. </summary>
    public event Func<SocketGuild, Task> JoinedGuild
    {
        add => _joinedGuildEvent.Add(value);
        remove => _joinedGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new();

    /// <summary> Fired when the connected account leaves a guild. </summary>
    public event Func<SocketGuild, Task> LeftGuild
    {
        add => _leftGuildEvent.Add(value);
        remove => _leftGuildEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new();

    /// <summary> Fired when a guild is updated. </summary>
    public event Func<SocketGuild, SocketGuild, Task> GuildUpdated
    {
        add => _guildUpdatedEvent.Add(value);
        remove => _guildUpdatedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new();

    /// <summary> Fired when a guild becomes available. </summary>
    public event Func<SocketGuild, Task> GuildAvailable
    {
        add => _guildAvailableEvent.Add(value);
        remove => _guildAvailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new();

    /// <summary> Fired when a guild becomes unavailable. </summary>
    public event Func<SocketGuild, Task> GuildUnavailable
    {
        add => _guildUnavailableEvent.Add(value);
        remove => _guildUnavailableEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new();

    #endregion

    #region Interactions

    /// <summary> Fired when a button is clicked in a card message. </summary>
    public event Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task> MessageButtonClicked
    {
        add => _messageButtonClickedEvent.Add(value);
        remove => _messageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task>> _messageButtonClickedEvent = new();

    /// <summary> Fired when a button is clicked in a direct card message. </summary>
    public event Func<string, SocketUser, IMessage, SocketDMChannel, Task> DirectMessageButtonClicked
    {
        add => _directMessageButtonClickedEvent.Add(value);
        remove => _directMessageButtonClickedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<string, SocketUser, IMessage, SocketDMChannel, Task>> _directMessageButtonClickedEvent = new();

    #endregion
}
