using Kook.Net.Udp;
using Kook.Net.WebSockets;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

#if DEBUG_AUDIO
using System.Diagnostics;
#endif

namespace Kook.Audio;

internal class KookVoiceAPIClient : IDisposable
{
    #region KookVoiceAPIClient

    public const int MaxBitrate = 500 * 1024;
    public static readonly DateTimeOffset PrimeEpoch = new(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);

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

    private readonly JsonSerializerOptions _serializerOptions;
    private readonly SemaphoreSlim _connectionLock;
    private readonly IUdpSocket _udp;
    private readonly IUdpSocket _rtcpUdp;
    private CancellationTokenSource? _connectCancellationToken;
    private bool _isDisposed;

    public ulong GuildId { get; }
    public ConnectionState ConnectionState { get; private set; }

    public ushort UdpPort => _udp.Port;

    public ushort RtcpUdpPort => _rtcpUdp.Port;

    internal KookVoiceAPIClient(ulong guildId, WebSocketProvider webSocketProvider,
        UdpSocketProvider udpSocketProvider, JsonSerializerOptions? serializerOptions = null)
    {
        GuildId = guildId;
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

        _serializerOptions = serializerOptions
            ?? new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
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

    private async Task SendRtcpAsync(byte[] data, int offset, int bytes)
    {
        await _rtcpUdp.SendAsync(data, offset, bytes).ConfigureAwait(false);
    }

    #endregion

    #region WebSocket

    public async Task SendRtcpAsync(uint ssrc, uint rtpTimestamp, uint sentPackets, uint sentOctets,
        RequestOptions? options = null)
    {
        byte[] packet = new byte[28];
        // 10.. .... = Version: RFC 1889 Version (2)
        // ..0. .... = Padding: False
        // ...0 0000 = Reception report count: 0
        packet[0] = 0b_10_0_00000;
        // Packet type: Sender Report (200)
        packet[1] = 0xc8;
        // Length: 6 (28 bytes)
        packet[2] = 0x00;
        packet[3] = 0x06;
        // Sender SSRC
        packet[4] = (byte)(ssrc >> 24);
        packet[5] = (byte)(ssrc >> 16);
        packet[6] = (byte)(ssrc >> 8);
        packet[7] = (byte)(ssrc >> 0);
        // NTP timestamp MSW
        DateTimeOffset now = DateTimeOffset.UtcNow;
        double secondsSinceEpoch = (now - PrimeEpoch).TotalSeconds;
        uint seconds = (uint)secondsSinceEpoch;
        packet[8] = (byte)(seconds >> 24);
        packet[9] = (byte)(seconds >> 16);
        packet[10] = (byte)(seconds >> 8);
        packet[11] = (byte)(seconds >> 0);
        // NTP timestamp LSW
        uint fraction = (uint)((secondsSinceEpoch - seconds) * uint.MaxValue);
        packet[12] = (byte)(fraction >> 24);
        packet[13] = (byte)(fraction >> 16);
        packet[14] = (byte)(fraction >> 8);
        packet[15] = (byte)(fraction >> 0);
        // RTP timestamp
        packet[16] = (byte)(rtpTimestamp >> 24);
        packet[17] = (byte)(rtpTimestamp >> 16);
        packet[18] = (byte)(rtpTimestamp >> 8);
        packet[19] = (byte)(rtpTimestamp >> 0);
        // Sender's packet count
        packet[20] = (byte)(sentPackets >> 24);
        packet[21] = (byte)(sentPackets >> 16);
        packet[22] = (byte)(sentPackets >> 8);
        packet[23] = (byte)(sentPackets >> 0);
        // Sender's octet count
        packet[24] = (byte)(sentOctets >> 24);
        packet[25] = (byte)(sentOctets >> 16);
        packet[26] = (byte)(sentOctets >> 8);
        packet[27] = (byte)(sentOctets >> 0);

        await SendRtcpAsync(packet, 0, 28).ConfigureAwait(false);
    }

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
