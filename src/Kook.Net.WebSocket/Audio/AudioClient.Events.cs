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
}
