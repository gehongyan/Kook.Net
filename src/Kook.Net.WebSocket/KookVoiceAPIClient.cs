using Kook.Net.Udp;

namespace Kook.Audio;

internal class KookVoiceAPIClient : IDisposable
{
    #region KookVoiceAPIClient

    public const int MaxBitrate = 500 * 1024;

    public event Func<string, string, double, Task> SentRequest
    {
        add => _sentRequestEvent.Add(value);
        remove => _sentRequestEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new();

    public event Func<int, Task> SentData
    {
        add => _sentDataEvent.Add(value);
        remove => _sentDataEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<int, Task>> _sentDataEvent = new();

    public event Func<byte[], Task> ReceivedPacket
    {
        add => _receivedPacketEvent.Add(value);
        remove => _receivedPacketEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<byte[], Task>> _receivedPacketEvent = new();

    public event Func<byte[], Task> ReceivedRtcpPacket
    {
        add => _receivedRtcpPacketEvent.Add(value);
        remove => _receivedRtcpPacketEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<byte[], Task>> _receivedRtcpPacketEvent = new();

    public event Func<Exception, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new();

    private readonly SemaphoreSlim _connectionLock;
    private readonly IUdpSocket _udp;
    private readonly IUdpSocket _rtcpUdp;
    private CancellationTokenSource? _connectCancellationToken;
    private bool _isDisposed;

    public ConnectionState ConnectionState { get; private set; }

    public ushort UdpPort => _udp.Port;

    public ushort RtcpUdpPort => _rtcpUdp.Port;

    internal KookVoiceAPIClient(UdpSocketProvider udpSocketProvider)
    {
        _connectionLock = new SemaphoreSlim(1, 1);
        _udp = udpSocketProvider();
        _udp.ReceivedDatagram += async (data, index, count) =>
        {
            if (index != 0 || count != data.Length)
            {
                byte[] newData = new byte[count];
                Buffer.BlockCopy(data, index, newData, 0, count);
                data = newData;
            }

            await _receivedPacketEvent.InvokeAsync(data).ConfigureAwait(false);
        };
        _rtcpUdp = udpSocketProvider();
        _rtcpUdp.ReceivedDatagram += async (data, index, count) =>
        {
            if (index != 0 || count != data.Length)
            {
                byte[] newData = new byte[count];
                Buffer.BlockCopy(data, index, newData, 0, count);
                data = newData;
            }

            await _receivedRtcpPacketEvent.InvokeAsync(data).ConfigureAwait(false);
        };
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _connectCancellationToken?.Dispose();
            _udp?.Dispose();
            _rtcpUdp?.Dispose();

            _connectionLock?.Dispose();
        }

        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);

    public async Task SendAsync(byte[] data, int offset, int bytes)
    {
        await _udp.SendAsync(data, offset, bytes).ConfigureAwait(false);
        await _sentDataEvent.InvokeAsync(bytes).ConfigureAwait(false);
    }

    public async Task SendRtcpAsync(byte[] data, int offset, int bytes)
    {
        await _rtcpUdp.SendAsync(data, offset, bytes).ConfigureAwait(false);
    }

    #endregion

    #region WebSocket

    public async Task ConnectAsync()
    {
        await _connectionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await ConnectInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task ConnectInternalAsync()
    {
        ConnectionState = ConnectionState.Connecting;
        try
        {
            _connectCancellationToken?.Dispose();
            _connectCancellationToken = new CancellationTokenSource();
            CancellationToken cancellationToken = _connectCancellationToken.Token;

            _udp.SetCancellationToken(cancellationToken);
            _rtcpUdp.SetCancellationToken(cancellationToken);
            await _udp.StartAsync().ConfigureAwait(false);
            await _rtcpUdp.StartAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Connected;
        }
        catch
        {
            await DisconnectInternalAsync().ConfigureAwait(false);
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        await _connectionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task DisconnectInternalAsync()
    {
        if (ConnectionState == ConnectionState.Disconnected) return;

        ConnectionState = ConnectionState.Disconnecting;

        try
        {
            _connectCancellationToken?.Cancel(false);
        }
        catch
        {
            // ignored
        }

        //Wait for tasks to complete
        await _udp.StopAsync().ConfigureAwait(false);
        await _rtcpUdp.StopAsync().ConfigureAwait(false);

        ConnectionState = ConnectionState.Disconnected;
    }

    #endregion

    #region Udp

    public void SetUdpEndpoint(string ip, int port) => _udp.SetDestination(ip, port);

    public void SetRtcpUdpEndpoint(string ip, int port) => _rtcpUdp.SetDestination(ip, port);

    #endregion
}
