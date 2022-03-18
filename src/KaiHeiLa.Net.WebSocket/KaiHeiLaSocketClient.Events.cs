using System;
using System.Threading.Tasks;
using KaiHeiLa.API;

namespace KaiHeiLa.WebSocket
{
    public partial class KaiHeiLaSocketClient
    {
        #region General
        /// <summary> Fired when connected to the KaiHeiLa gateway. </summary>
        public event Func<Task> Connected
        {
            add => _connectedEvent.Add(value);
            remove => _connectedEvent.Remove(value);
        }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        /// <summary> Fired when disconnected to the KaiHeiLa gateway. </summary>
        public event Func<Exception, Task> Disconnected
        {
            add => _disconnectedEvent.Add(value);
            remove => _disconnectedEvent.Remove(value);
        }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        /// <summary>
        ///     Fired when guild data has finished downloading.
        /// </summary>
        public event Func<Task> Ready
        {
            add => _readyEvent.Add(value);
            remove => _readyEvent.Remove(value);
        }
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();
        /// <summary> Fired when a heartbeat is received from the KaiHeiLa gateway. </summary>
        public event Func<int, int, Task> LatencyUpdated
        {
            add => _latencyUpdatedEvent.Add(value);
            remove => _latencyUpdatedEvent.Remove(value);
        }
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

        #endregion

        #region Guilds
        /// <summary> Fired when a guild becomes available. </summary>
        public event Func<SocketGuild, Task> GuildAvailable
        {
            add => _guildAvailableEvent.Add(value);
            remove => _guildAvailableEvent.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes unavailable. </summary>
        public event Func<SocketGuild, Task> GuildUnavailable
        {
            add => _guildUnavailableEvent.Add(value);
            remove => _guildUnavailableEvent.Remove(value);
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();

        #endregion

        #region Channels

        /// <summary> Fired when a reaction is added to a channel message. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a reaction is added to a user message in a channel. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an 
        ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
        ///         <see cref="Cacheable{TEntity,TId}"/> entity will contain the original message; otherwise, in event
        ///         that the message cannot be retrieved, the ID of the message is preserved in the 
        ///         <see cref="ulong"/>.
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
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionAddedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>>();
        /// <summary> Fired when a reaction is removed from a message. </summary>
        public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> ReactionRemoved
        {
            add => _reactionRemovedEvent.Add(value);
            remove => _reactionRemovedEvent.Remove(value);
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> _reactionRemovedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>>();
        
        
        /// <summary> Fired when a reaction is added to a direct message. </summary>
        /// <remarks>
        ///     <para>
        ///         This event is fired when a reaction is added to a user message in a private channel. The event handler must return a
        ///         <see cref="Task"/> and accept a <see cref="Cacheable{TEntity,TId}"/>, an 
        ///         <see cref="ISocketMessageChannel"/>, and a <see cref="SocketReaction"/> as its parameter.
        ///     </para>
        ///     <para>
        ///         If caching is enabled via <see cref="KaiHeiLaSocketConfig"/>, the
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
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionAddedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>>();
        /// <summary> Fired when a reaction is removed from a message. </summary>
        public event Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task> DirectReactionRemoved
        {
            add => _directReactionRemovedEvent.Add(value);
            remove => _directReactionRemovedEvent.Remove(value);
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>> _directReactionRemovedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, Guid>, Cacheable<IDMChannel, Guid>, SocketReaction, Task>>();

        #endregion

        #region Interactions

        /// <summary> Fired when a button is clicked in a card message. </summary>
        public event Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task> MessageButtonClicked
        {
            add => _messageButtonClickedEvent.Add(value);
            remove => _messageButtonClickedEvent.Remove(value);
        }
        internal readonly AsyncEvent<Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task>> _messageButtonClickedEvent = new AsyncEvent<Func<string, SocketUser, IMessage, SocketTextChannel, SocketGuild, Task>>();

        #endregion
    }
}
