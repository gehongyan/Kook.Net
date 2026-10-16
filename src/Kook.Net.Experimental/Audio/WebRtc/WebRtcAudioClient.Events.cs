namespace Kook.Audio.WebRtc;

internal sealed partial class WebRtcAudioClient
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

    public event Func<int, int, Task> UdpLatencyUpdated
    {
        add => _udpLatencyUpdatedEvent.Add(value);
        remove => _udpLatencyUpdatedEvent.Remove(value);
    }

    // WebRTC 传输不经过官方的 UDP 语音服务器，此事件不会被引发，仅为实现 IAudioClient 而保留。
    private readonly AsyncEvent<Func<int, int, Task>> _udpLatencyUpdatedEvent = new();

    public event Func<uint, AudioInStream, Task> StreamCreated
    {
        add => _streamCreatedEvent.Add(value);
        remove => _streamCreatedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<uint, AudioInStream, Task>> _streamCreatedEvent = new();

    public event Func<uint, Task> StreamDestroyed
    {
        add => _streamDestroyedEvent.Add(value);
        remove => _streamDestroyedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<uint, Task>> _streamDestroyedEvent = new();

    public event Func<VoicePeer, Task> PeerJoined
    {
        add => _peerJoinedEvent.Add(value);
        remove => _peerJoinedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<VoicePeer, Task>> _peerJoinedEvent = new();

    public event Func<ulong, Task> PeerLeft
    {
        add => _peerLeftEvent.Add(value);
        remove => _peerLeftEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _peerLeftEvent = new();

    public event Func<ulong, bool, Task> PeerMicrophoneMuteChanged
    {
        add => _peerMicrophoneMuteChangedEvent.Add(value);
        remove => _peerMicrophoneMuteChangedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, bool, Task>> _peerMicrophoneMuteChangedEvent = new();

    public event Func<ulong, bool, Task> PeerHeadsetMuteChanged
    {
        add => _peerHeadsetMuteChangedEvent.Add(value);
        remove => _peerHeadsetMuteChangedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, bool, Task>> _peerHeadsetMuteChangedEvent = new();

    public event Func<ulong, VoicePeerPermissions, Task> PeerPermissionChanged
    {
        add => _peerPermissionChangedEvent.Add(value);
        remove => _peerPermissionChangedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, VoicePeerPermissions, Task>> _peerPermissionChangedEvent = new();

    public event Func<ulong, ulong?, Task> AtmosphereReceived
    {
        add => _atmosphereReceivedEvent.Add(value);
        remove => _atmosphereReceivedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, ulong?, Task>> _atmosphereReceivedEvent = new();

    public event Func<ulong, VoiceAccompaniment, Task> AccompanimentStarted
    {
        add => _accompanimentStartedEvent.Add(value);
        remove => _accompanimentStartedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, VoiceAccompaniment, Task>> _accompanimentStartedEvent = new();

    public event Func<ulong, Task> AccompanimentStopped
    {
        add => _accompanimentStoppedEvent.Add(value);
        remove => _accompanimentStoppedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<ulong, Task>> _accompanimentStoppedEvent = new();

    public event Func<int?, string?, Task> ServerDisconnected
    {
        add => _serverDisconnectedEvent.Add(value);
        remove => _serverDisconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<int?, string?, Task>> _serverDisconnectedEvent = new();

    public event Func<Task> ClosedByOtherDevice
    {
        add => _closedByOtherDeviceEvent.Add(value);
        remove => _closedByOtherDeviceEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _closedByOtherDeviceEvent = new();

    public event Func<Task> ReloadRequested
    {
        add => _reloadRequestedEvent.Add(value);
        remove => _reloadRequestedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _reloadRequestedEvent = new();

    // 信令连接被服务端关闭或踢出时引发，用于驱动 ConnectionManager 进入断开流程。
    private readonly AsyncEvent<Func<Exception, Task>> _signalingClosedEvent = new();
}
