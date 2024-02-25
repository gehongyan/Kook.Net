namespace Kook.Audio;

internal partial class AudioClient
{
    public event Func<Task> Connected
    {
        add { _connectedEvent.Add(value); }
        remove { _connectedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();

    public event Func<Exception, Task> Disconnected
    {
        add { _disconnectedEvent.Add(value); }
        remove { _disconnectedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

    // public event Func<int, int, Task> LatencyUpdated
    // {
    //     add { _latencyUpdatedEvent.Add(value); }
    //     remove { _latencyUpdatedEvent.Remove(value); }
    // }
    //
    // private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

    public event Func<int, int, Task> UdpLatencyUpdated
    {
        add { _udpLatencyUpdatedEvent.Add(value); }
        remove { _udpLatencyUpdatedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<int, int, Task>> _udpLatencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

    public event Func<ulong, Task> PeerConnected
    {
        add { _peerConnectedEvent.Add(value); }
        remove { _peerConnectedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<ulong, Task>> _peerConnectedEvent = new AsyncEvent<Func<ulong, Task>>();

    public event Func<ulong, Task> PeerDisconnected
    {
        add { _peerDisconnectedEvent.Add(value); }
        remove { _peerDisconnectedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<ulong, Task>> _peerDisconnectedEvent = new AsyncEvent<Func<ulong, Task>>();

    public event Func<Task> ClientDisconnected
    {
        add { _clientDisconnectedEvent.Add(value); }
        remove { _clientDisconnectedEvent.Remove(value); }
    }

    private readonly AsyncEvent<Func<Task>> _clientDisconnectedEvent = new AsyncEvent<Func<Task>>();
}
