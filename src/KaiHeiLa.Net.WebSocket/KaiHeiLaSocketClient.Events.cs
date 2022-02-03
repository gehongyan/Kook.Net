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
            add { _guildAvailableEvent.Add(value); }
            remove { _guildAvailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes unavailable. </summary>
        public event Func<SocketGuild, Task> GuildUnavailable
        {
            add { _guildUnavailableEvent.Add(value); }
            remove { _guildUnavailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();

        #endregion
    }
}
