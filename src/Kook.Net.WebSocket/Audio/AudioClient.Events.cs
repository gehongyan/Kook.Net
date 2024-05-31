namespace Kook.Audio;

internal partial class AudioClient
{
    public event Func<Task> Connected
    {
        add => _connectedEvent.Add(value);
        remove => _connectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _connectedEvent = new();

    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new();

    // public event Func<int, int, Task> LatencyUpdated
    // {
    //     add { _latencyUpdatedEvent.Add(value); }
    //     remove { _latencyUpdatedEvent.Remove(value); }
    // }
    //
    // private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

    public event Func<int, int, Task> UdpLatencyUpdated
    {
        add => _udpLatencyUpdatedEvent.Add(value);
        remove => _udpLatencyUpdatedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<int, int, Task>> _udpLatencyUpdatedEvent = new();

    public event Func<ulong, Task> PeerConnected
    {
        add => _peerConnectedEvent.Add(value);
        remove => _peerConnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _peerConnectedEvent = new();

    public event Func<ulong, Task> PeerDisconnected
    {
        add => _peerDisconnectedEvent.Add(value);
        remove => _peerDisconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _peerDisconnectedEvent = new();

    public event Func<ulong, Task> HeadsetResumed
    {
        add => _headsetResumedEvent.Add(value);
        remove => _headsetResumedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _headsetResumedEvent = new();

    public event Func<ulong, Task> HeadsetPaused
    {
        add => _headsetPausedEvent.Add(value);
        remove => _headsetPausedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _headsetPausedEvent = new();

    public event Func<ulong, Task> ConsumerResumed
    {
        add => _consumerResumedEvent.Add(value);
        remove => _consumerResumedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _consumerResumedEvent = new();

    public event Func<ulong, Task> ConsumerPaused
    {
        add => _consumerPausedEvent.Add(value);
        remove => _consumerPausedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _consumerPausedEvent = new();

    public event Func<ulong, PeerPermissionInfo, Task> PeerPermissionChanged
    {
        add => _peerPermissionChangedEvent.Add(value);
        remove => _peerPermissionChangedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, PeerPermissionInfo, Task>> _peerPermissionChangedEvent = new();

    public event Func<ulong, int, Task> AtmospherePlayed
    {
        add => _atmospherePlayedEvent.Add(value);
        remove => _atmospherePlayedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, int, Task>> _atmospherePlayedEvent = new();

    public event Func<ulong, SoundtrackInfo, Task> SoundtrackStarted
    {
        add => _soundtrackStartedEvent.Add(value);
        remove => _soundtrackStartedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, SoundtrackInfo, Task>> _soundtrackStartedEvent = new();

    public event Func<ulong, Task> SoundtrackStopped
    {
        add => _soundtrackStoppedEvent.Add(value);
        remove => _soundtrackStoppedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _soundtrackStoppedEvent = new();

    public event Func<Task> ClientDisconnected
    {
        add => _clientDisconnectedEvent.Add(value);
        remove => _clientDisconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _clientDisconnectedEvent = new();
}
