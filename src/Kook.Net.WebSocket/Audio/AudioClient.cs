using Kook.Logging;
using Kook.WebSocket;
using Kook.API.Rest;
using Kook.Audio.Streams;

namespace Kook.Audio;

internal partial class AudioClient : IAudioClient
{
    private const int ConnectionTimeoutMs = 30000; // 30 seconds
    public static readonly DateTimeOffset PrimeEpoch = new(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly Random _ssrcRandom;

    private readonly string? _password;
    private readonly Logger _audioLogger;
    private readonly SemaphoreSlim _stateLock;

    private Task? /*_heartbeatTask, _keepaliveTask, */_rtcpTask;
    private long _lastRtcpTime;
    private uint _ssrc;
    private byte _payloadType;
    private int _bitrate;

    internal ConnectionManager Connection { get; }

    internal uint LastRtpTimestamp { get; set; }
    internal ushort LastRtpSequence { get; set; }
    internal uint SentPackets { get; set; }
    internal uint SentOctets { get; set; }
    internal int LastRtpActiveTick { get; set; }

    public SocketVoiceChannel Channel { get; }

    public KookVoiceAPIClient ApiClient { get; }
    // public int Latency { get; private set; }
    public int UdpLatency { get; private set; }

    public ulong ChannelId => Channel.Id;
    public SocketGuild Guild => Channel.Guild;
    private KookSocketClient Kook => Guild.Kook;

    public ConnectionState ConnectionState => Connection.State;

    /// <summary> Creates a new REST/WebSocket kook client. </summary>
    internal AudioClient(SocketVoiceChannel voiceChannel, int clientId, string? password)
    {
        Channel = voiceChannel;
        _password = password;
        _ssrcRandom = new Random();
        _audioLogger = Kook.LogManager.CreateLogger($"Audio #{clientId}");

        ApiClient = new KookVoiceAPIClient(Kook.UdpSocketProvider);
        // ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
        ApiClient.ReceivedPacket += ProcessPacketAsync;
        ApiClient.ReceivedRtcpPacket += ProcessRtcpPacketAsync;

        _stateLock = new SemaphoreSlim(1, 1);
        Connection = new ConnectionManager(_stateLock, _audioLogger, ConnectionTimeoutMs,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        Connection.Connected += () => _connectedEvent.InvokeAsync();
        Connection.Disconnected += (ex, recon) => _disconnectedEvent.InvokeAsync(ex);

        UdpLatencyUpdated += async (old, val) =>
            await _audioLogger.DebugAsync($"UDP Latency = {val} ms").ConfigureAwait(false);
    }

    internal Task StartAsync() => Connection.StartAsync();

    public Task StopAsync() => Connection.StopAsync();

    private async Task OnConnectingAsync()
    {
        await _audioLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
        _ssrc = (uint)_ssrcRandom.Next(0, int.MaxValue);
        _payloadType = 0x6F;
        _bitrate = 96 * 1024;
        CreateVoiceGatewayParams args = new()
        {
            ChannelId = ChannelId,
            Ssrc = _ssrc,
            PayloadType = _payloadType,
            RtcpMultiplexing = false,
            Password = _password
        };
        CreateVoiceGatewayResponse voiceGatewayResponse = await Kook.ApiClient.CreateVoiceGatewayAsync(args).ConfigureAwait(false);
        _ssrc = voiceGatewayResponse.Ssrc;
        _payloadType = voiceGatewayResponse.PayloadType;
        _bitrate = voiceGatewayResponse.Bitrate;
        ApiClient.SetUdpEndpoint(voiceGatewayResponse.Ip, voiceGatewayResponse.Port);
        ApiClient.SetRtcpUdpEndpoint(voiceGatewayResponse.Ip, voiceGatewayResponse.RtcpPort);
        _rtcpTask = RunRtcpAsync(KookSocketConfig.RtcpIntervalMilliseconds, Connection.CancellationToken);
        await ApiClient.ConnectAsync().ConfigureAwait(false);
        await _audioLogger.DebugAsync($"Listening on port {ApiClient.UdpPort}, {ApiClient.RtcpUdpPort}").ConfigureAwait(false);

        await Connection.CompleteAsync();
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _audioLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);

        await ApiClient.DisconnectAsync().ConfigureAwait(false);

        if (_rtcpTask != null)
            await _rtcpTask.ConfigureAwait(false);
        _rtcpTask = null;
    }

    /// <inheritdoc />
    public AudioOutStream CreateOpusStream(int bufferMillis = 1000)
    {
        OutputStream outputStream = new(ApiClient);                                                                     //Ignores header
        RtpWriteStream rtpWriter = new(outputStream, _ssrc, _payloadType);                                              //Consumes header, passes
        return new BufferedWriteStream(rtpWriter, this, bufferMillis, Connection.CancellationToken, _audioLogger); //Generates header
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectOpusStream()
    {
        OutputStream outputStream = new(ApiClient);                   //Ignores header
        return new RtpWriteStream(outputStream, _ssrc, _payloadType); //Consumes header, passes
    }

    /// <inheritdoc />
    public AudioOutStream CreatePcmStream(AudioApplication application, int? bitrate = null, int bufferMillis = 1000, int packetLoss = 30)
    {
        OutputStream outputStream = new(ApiClient);
        RtpWriteStream rtpWriter = new(outputStream, _ssrc, _payloadType);
        BufferedWriteStream bufferedStream = new(rtpWriter, this, bufferMillis, Connection.CancellationToken, _audioLogger); //Ignores header, generates header
        return new OpusEncodeStream(bufferedStream, bitrate ?? _bitrate, application, packetLoss);
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectPcmStream(AudioApplication application, int? bitrate = null, int packetLoss = 30)
    {
        OutputStream outputStream = new(ApiClient);
        RtpWriteStream rtpWriter = new(outputStream, _ssrc, _payloadType);
        return new OpusEncodeStream(rtpWriter, bitrate ?? _bitrate, application, packetLoss);
    }

    private async Task ProcessPacketAsync(byte[] packet)
    {
        try
        {
            // Print packets
            await _audioLogger.DebugAsync($"Received {packet.Length} Bytes").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _audioLogger.WarningAsync("Failed to process UDP packet", ex).ConfigureAwait(false);
        }
    }

    private async Task ProcessRtcpPacketAsync(byte[] packet)
    {
        try
        {
            if (_lastRtcpTime == 0)
                return;

            int before = UdpLatency;
            long now = Environment.TickCount;
            double dlsr = ((packet[28] << 8) | packet[29])
                + (double)((packet[30] << 8) | packet[31]) / ushort.MaxValue;
            double delay = now - dlsr * 1000 - _lastRtcpTime;

            _ = _audioLogger.DebugAsync($"Received RTCP {packet.Length} Bytes").ConfigureAwait(false);

            if (delay < 0)
                return;

            UdpLatency = (int)delay;
            await _udpLatencyUpdatedEvent.InvokeAsync(before, UdpLatency).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _audioLogger.WarningAsync("Failed to process RTCP packet", ex).ConfigureAwait(false);
        }
    }

    private async Task RunRtcpAsync(int intervalMillis, CancellationToken cancellationToken)
    {
        await Task.Yield();
        try
        {
            await _audioLogger.DebugAsync("RTCP Started").ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                if (LastRtpTimestamp == 0 || SentPackets == 0 || SentOctets == 0)
                {
                    await Task.Delay(intervalMillis, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                _lastRtcpTime = Environment.TickCount;

                try
                {
                    await SendRtcpAsync(_ssrc, LastRtpTimestamp, SentPackets, SentOctets).ConfigureAwait(false);
#if DEBUG
                    _ = _audioLogger.DebugAsync("Sent RTCP").ConfigureAwait(false);
#endif
                }
                catch (Exception ex)
                {
                    await _audioLogger.WarningAsync("Failed to send RTCP", ex).ConfigureAwait(false);
                }

                int delay = Math.Max(0, intervalMillis - UdpLatency);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            await _audioLogger.DebugAsync("RTCP Stopped").ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await _audioLogger.DebugAsync("RTCP Stopped").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _audioLogger.ErrorAsync("RTCP Errored", ex).ConfigureAwait(false);
        }
    }

    private async Task SendRtcpAsync(uint ssrc, uint rtpTimestamp, uint sentPackets, uint sentOctets)
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

        await ApiClient.SendRtcpAsync(packet, 0, 28).ConfigureAwait(false);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAsync().GetAwaiter().GetResult();
            ApiClient.Dispose();
            _stateLock?.Dispose();
        }
    }
    /// <inheritdoc />
    public void Dispose() => Dispose(true);
}
