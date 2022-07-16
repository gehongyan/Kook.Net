using KaiHeiLa.API;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

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
    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelCreatedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
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
    internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelDestroyedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
    /// <summary> Fired when a channel is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a generic channel has been destroyed. The event handler must return a
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
    internal readonly AsyncEvent<Func<SocketChannel, SocketChannel, Task>> _channelUpdatedEvent = new AsyncEvent<Func<SocketChannel, SocketChannel, Task>>();
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
    internal readonly AsyncEvent<Func<SocketMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<SocketMessage, Task>>();
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
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the message cannot be retrieved by KaiHeiLa
    ///             after the message has been deleted.
    ///         </note>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessageChannel, ulong>, Task>> _messageDeletedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IMessageChannel, ulong>, Task>>();
    
    /// <summary> Fired when a message is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, Task>> _messageUpdatedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, Task>>();

    /// <summary> Fired when a message is pinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is pinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>> _messagePinnedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>>();
    
    /// <summary> Fired when a message is unpinned. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is unpinned. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="ISocketMessageChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>> _messageUnpinnedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, ISocketMessageChannel, SocketGuildUser, Task>>();

    
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
    internal readonly AsyncEvent<Func<SocketMessage, Task>> _directMessageReceivedEvent = new AsyncEvent<Func<SocketMessage, Task>>();
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
    ///             <see cref="Cacheable{TEntity,TId}.DownloadAsync"/>; the message cannot be retrieved by KaiHeiLa
    ///             after the message has been deleted.
    ///         </note>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, Task>> _directMessageDeletedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, Cacheable<IDMChannel, Guid>, Task>>();
    
    /// <summary> Fired when a message is updated. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is updated. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, <see cref="SocketMessage"/>,
    ///         and <see cref="IDMChannel"/> as its parameters.
    ///     </para>
    ///     <para>
    ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
    internal readonly AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, IDMChannel, Task>> _directMessageUpdatedEvent = new AsyncEvent<Func<Cacheable<IMessage, Guid>, SocketMessage, IDMChannel, Task>>();

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
    internal readonly AsyncEvent<Func<SocketGuildUser, DateTimeOffset, Task>> _userJoinedEvent = new AsyncEvent<Func<SocketGuildUser, DateTimeOffset, Task>>();
    /// <summary> Fired when a user leaves a guild. </summary>
    public event Func<SocketGuild, SocketUser, DateTimeOffset, Task> UserLeft
    {
        add => _userLeftEvent.Add(value);
        remove => _userLeftEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketGuild, SocketUser,DateTimeOffset, Task>> _userLeftEvent = new AsyncEvent<Func<SocketGuild, SocketUser, DateTimeOffset, Task>>();
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
    /// </remarks>
    public event Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task> UserBanned
    {
        add => _userBannedEvent.Add(value);
        remove => _userBannedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task>> _userBannedEvent = new AsyncEvent<Func<IReadOnlyCollection<SocketUser>, SocketUser, SocketGuild, Task>>();
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
    public event Func<SocketUser, SocketGuild, Task> UserUnbanned
    {
        add => _userUnbannedEvent.Add(value);
        remove => _userUnbannedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketUser, SocketGuild, Task>> _userUnbannedEvent = new AsyncEvent<Func<SocketUser, SocketGuild, Task>>();
    /// <summary> Fired when a user is updated. </summary>
    public event Func<SocketUser, SocketUser, Task> UserUpdated
    {
        add => _userUpdatedEvent.Add(value);
        remove => _userUpdatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketUser, SocketUser, Task>> _userUpdatedEvent = new AsyncEvent<Func<SocketUser, SocketUser, Task>>();
    /// <summary> Fired when the connected account is updated. </summary>
    public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated
    {
        add => _selfUpdatedEvent.Add(value);
        remove => _selfUpdatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _selfUpdatedEvent = new AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>>();
    /// <summary> Fired when a guild member is updated. </summary>
    public event Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task> GuildMemberUpdated
    {
        add => _guildMemberUpdatedEvent.Add(value);
        remove => _guildMemberUpdatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>> _guildMemberUpdatedEvent = new AsyncEvent<Func<Cacheable<SocketGuildUser, ulong>, SocketGuildUser, Task>>();
    /// <summary> Fired when a guild member is online. </summary>
    public event Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task> GuildMemberOnline
    {
        add => _guildMemberOnlineEvent.Add(value);
        remove => _guildMemberOnlineEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>> _guildMemberOnlineEvent = new AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>>();
    /// <summary> Fired when a guild member is offline. </summary>
    public event Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task> GuildMemberOffline
    {
        add => _guildMemberOfflineEvent.Add(value);
        remove => _guildMemberOfflineEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>> _guildMemberOfflineEvent = new AsyncEvent<Func<IReadOnlyCollection<SocketGuildUser>, DateTimeOffset, Task>>();
    
    #endregion

    #region Voices

    /// <summary> Fired when a user connected to a voice channel. </summary>
    public event Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task> UserConnected
    {
        add => _userConnectedEvent.Add(value);
        remove => _userConnectedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>> _userConnectedEvent = new AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>>();
    /// <summary> Fired when a user disconnected to a voice channel. </summary>
    public event Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task> UserDisconnected
    {
        add => _userDisconnectedEvent.Add(value);
        remove => _userDisconnectedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>> _userDisconnectedEvent = new AsyncEvent<Func<SocketUser, SocketVoiceChannel, SocketGuild, DateTimeOffset, Task>>();

    #endregion

    #region Roles

    public event Func<SocketRole, Task> RoleCreated
    {
        add => _roleCreatedEvent.Add(value);
        remove => _roleCreatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new AsyncEvent<Func<SocketRole, Task>>();
    /// <summary> Fired when a role is deleted. </summary>
    public event Func<SocketRole, Task> RoleDeleted
    {
        add => _roleDeletedEvent.Add(value);
        remove => _roleDeletedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new AsyncEvent<Func<SocketRole, Task>>();
    /// <summary> Fired when a role is updated. </summary>
    public event Func<SocketRole, SocketRole, Task> RoleUpdated
    {
        add => _roleUpdatedEvent.Add(value);
        remove => _roleUpdatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new AsyncEvent<Func<SocketRole, SocketRole, Task>>();

    #endregion

    #region Guilds
    
    /// <summary> Fired when the connected account joins a guild. </summary>
    public event Func<SocketGuild, Task> JoinedGuild
    {
        add => _joinedGuildEvent.Add(value);
        remove => _joinedGuildEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
    /// <summary> Fired when the connected account leaves a guild. </summary>
    public event Func<SocketGuild, Task> LeftGuild
    {
        add => _leftGuildEvent.Add(value);
        remove => _leftGuildEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
    /// <summary> Fired when a guild is updated. </summary>
    public event Func<SocketGuild, SocketGuild, Task> GuildUpdated
    {
        add => _guildUpdatedEvent.Add(value);
        remove => _guildUpdatedEvent.Remove(value);
    }
    internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new AsyncEvent<Func<SocketGuild, SocketGuild, Task>>();

    #endregion
}