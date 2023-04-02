namespace Kook.WebSocket;

public partial class KookSocketClient
{
    #region General

    /// <summary> Fired when connected to the Kook gateway. </summary>
    public event Func<Task> Connected
    {
        add => _connectedEvent.Add(value);
        remove => _connectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _connectedEvent = new();

    /// <summary> Fired when disconnected to the Kook gateway. </summary>
    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new();

    /// <summary>
    ///     Fired when guild data has finished downloading.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         Because guilds may contain a large amount of members,
    ///         this event will not wait for all users, subscriptions, and voice states
    ///         to be downloaded. It will only wait for guilds, channels, roles, and
    ///         emojis to be downloaded.
    ///     </note>
    /// </remarks>
    public event Func<Task> Ready
    {
        add => _readyEvent.Add(value);
        remove => _readyEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _readyEvent = new();

    /// <summary> Fired when a heartbeat is received from the Kook gateway. </summary>
    public event Func<int, int, Task> LatencyUpdated
    {
        add => _latencyUpdatedEvent.Add(value);
        remove => _latencyUpdatedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new();

    #endregion
}
