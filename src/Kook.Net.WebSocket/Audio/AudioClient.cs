using Kook.API.Voice;
using Kook.Logging;
using Kook.WebSocket;
using System.Text.Json;
using Kook.API.Rest;
using Kook.Audio.Streams;

namespace Kook.Audio;

//TODO: Add audio reconnecting
internal partial class AudioClient : IAudioClient
{
    private const int ConnectionTimeoutMs = 30000; // 30 seconds

    private readonly Random _ssrcRandom;

    private readonly Logger _audioLogger;
    private readonly ConnectionManager _connection;
    private readonly SemaphoreSlim _stateLock;

    private Task? /*_heartbeatTask, _keepaliveTask, */_rtcpTask;
    private long _lastRtcpTime;
    private uint _sequence;
    private uint _ssrc;

    internal uint LastRtpTimestamp { get; set; }
    internal ushort LastRtpSequence { get; set; }

    internal uint SentPackets { get; set; }

    internal uint SentOctets { get; set; }

    public SocketGuild Guild { get; }
    public KookVoiceAPIClient ApiClient { get; }
    // public int Latency { get; private set; }
    public int UdpLatency { get; private set; }
    public ulong ChannelId { get; }

    private KookSocketClient Kook => Guild.Kook;

    public ConnectionState ConnectionState => _connection.State;

    /// <summary> Creates a new REST/WebSocket kook client. </summary>
    internal AudioClient(SocketGuild guild, int clientId, ulong channelId)
    {
        Guild = guild;
        ChannelId = channelId;
        _ssrcRandom = new Random();
        _audioLogger = Kook.LogManager.CreateLogger($"Audio #{clientId}");

        ApiClient = new KookVoiceAPIClient(guild.Id, Kook.WebSocketProvider, Kook.UdpSocketProvider);
        ApiClient.SentGatewayMessage += async opCode =>
            await _audioLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
        //ApiClient.SentData += async bytes => await _audioLogger.DebugAsync($"Sent {bytes} Bytes").ConfigureAwait(false);
        ApiClient.ReceivedEvent += ProcessMessageAsync;
        ApiClient.ReceivedPacket += ProcessPacketAsync;
        ApiClient.ReceivedRtcpPacket += ProcessRtcpPacketAsync;

        _stateLock = new SemaphoreSlim(1, 1);
        _connection = new ConnectionManager(_stateLock, _audioLogger, ConnectionTimeoutMs,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        _connection.Connected += () => _connectedEvent.InvokeAsync();
        _connection.Disconnected += (ex, recon) => _disconnectedEvent.InvokeAsync(ex);
        // _heartbeatTimes = new ConcurrentQueue<long>();
        _sequence = 1000000;

        // LatencyUpdated += async (old, val) => await _audioLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);
        UdpLatencyUpdated += async (old, val) =>
            await _audioLogger.DebugAsync($"UDP Latency = {val} ms").ConfigureAwait(false);
    }

    internal Task StartAsync() => _connection.StartAsync();

    public Task StopAsync() => _connection.StopAsync();

    private async Task OnConnectingAsync()
    {
        await _audioLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
        GetVoiceGatewayResponse voiceGatewayResponse = await Kook.ApiClient.GetVoiceGatewayAsync(ChannelId).ConfigureAwait(false);
        await ApiClient.ConnectAsync(voiceGatewayResponse.Url).ConfigureAwait(false);
        await _audioLogger.DebugAsync($"Listening on port {ApiClient.UdpPort}, {ApiClient.RtcpUdpPort}").ConfigureAwait(false);

        uint sequence = _sequence++;
        await ApiClient.SendGetRouterRtpCapabilitiesRequestAsync(sequence).ConfigureAwait(false);

        // // TODO: Wait for READY
        await _connection.WaitAsync().ConfigureAwait(false);
    }
    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _audioLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
        await ApiClient.DisconnectAsync().ConfigureAwait(false);

        // //Wait for tasks to complete
        // await _audioLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);

        // if (_heartbeatTask != null)
        //     await _heartbeatTask.ConfigureAwait(false);
        // _heartbeatTask = null;

        // if (_keepaliveTask != null)
        //     await _keepaliveTask.ConfigureAwait(false);
        // _keepaliveTask = null;

        if (_rtcpTask != null)
            await _rtcpTask.ConfigureAwait(false);
        _rtcpTask = null;

        // while (_heartbeatTimes.TryDequeue(out _))
        // {
        // }

        // await ClearInputStreamsAsync().ConfigureAwait(false);

        // await _audioLogger.DebugAsync("Sending Voice State").ConfigureAwait(false);
        // await Kook.ApiClient.SendVoiceStateUpdateAsync(Guild.Id, null, false, false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public AudioOutStream CreateOpusStream(int bufferMillis = 1000)
    {
        OutputStream outputStream = new(ApiClient);                                                                 //Ignores header
        RtpWriteStream rtpWriter = new(outputStream, _ssrc);                                                        //Consumes header, passes
        return new BufferedWriteStream(rtpWriter, this, bufferMillis, _connection.CancellationToken, _audioLogger); //Generates header
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectOpusStream()
    {
        OutputStream outputStream = new(ApiClient);     //Ignores header
        return new RtpWriteStream(outputStream, _ssrc); //Consumes header, passes
    }

    /// <inheritdoc />
    public AudioOutStream CreatePcmStream(AudioApplication application, int bitrate = 96 * 1024, int bufferMillis = 1000, int packetLoss = 30)
    {
        OutputStream outputStream = new(ApiClient);
        RtpWriteStream rtpWriter = new(outputStream, _ssrc);
        BufferedWriteStream bufferedStream = new(rtpWriter, this, bufferMillis, _connection.CancellationToken, _audioLogger); //Ignores header, generates header
        return new OpusEncodeStream(bufferedStream, bitrate, application, packetLoss);
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectPcmStream(AudioApplication application, int bitrate = 96 * 1024, int packetLoss = 30)
    {
        OutputStream outputStream = new(ApiClient);
        RtpWriteStream rtpWriter = new(outputStream, _ssrc);
        return new OpusEncodeStream(rtpWriter, bitrate, application, packetLoss);
    }

    private async Task ProcessMessageAsync(VoiceSocketFrameType type, bool okay, object payload)
    {
        if (!okay)
        {
            await _audioLogger.ErrorAsync($"Gateway {type} Failure: {payload}").ConfigureAwait(false);
            _connection.Error(new Exception("Voice Connection Failed"));
            return;
        }

        try
        {
            switch (type)
            {
                case VoiceSocketFrameType.GetRouterRtpCapabilities:
                {
                    await _audioLogger.DebugAsync("RouterRtpCapabilities Completed").ConfigureAwait(false);
                    uint nextSequence = _sequence++;
                    await ApiClient.SendJoinRequestAsync(nextSequence).ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.Join:
                {
                    await _audioLogger.DebugAsync("Join Completed").ConfigureAwait(false);
                    uint nextSequence = _sequence++;
                    await ApiClient.SendCreatePlainTransportRequestAsync(nextSequence).ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.CreatePlainTransport:
                {
                    await _audioLogger.DebugAsync("CreatePlainTransport Completed").ConfigureAwait(false);
                    string? json = payload.ToString();
                    if (json is null
                        || JsonSerializer.Deserialize<CreatePlainTransportResponse>(json) is not { } data)
                    {
                        _connection.Error(new Exception($"Unable to parse CreatePlainTransportResponse: {json}"));
                        break;
                    }
                    ApiClient.SetUdpEndpoint(data.Ip, data.Port);
                    ApiClient.SetRtcpUdpEndpoint(data.Ip, data.RtcpPort);
                    uint nextSequence = _sequence++;
                    _ssrc = (uint)_ssrcRandom.Next(0, int.MaxValue);
                    if (Kook.CurrentUser is null)
                    {
                        _connection.CriticalError(new Exception("The client is not logged in"));
                        break;
                    }
                    await ApiClient
                        .SendProduceRequestAsync(nextSequence, Kook.CurrentUser.Id, data.Id, _ssrc)
                        .ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.Produce:
                {
                    await _audioLogger.DebugAsync("Produce Completed").ConfigureAwait(false);
                    _ = _connection.CompleteAsync();
                    // int intervalMillis = KookSocketConfig.HeartbeatIntervalMilliseconds;
                    // _heartbeatTask = RunHeartbeatAsync(intervalMillis, _connection.CancellationToken);
                    // _keepaliveTask = RunKeepaliveAsync(_connection.CancellationToken);
                    _rtcpTask = RunRtcpAsync(KookSocketConfig.RtcpIntervalMilliseconds, _connection.CancellationToken);
                }
                    break;
                case VoiceSocketFrameType.NewPeer:
                {
                    if (payload is not JsonElement jsonElement
                        || !jsonElement.TryGetProperty("id", out JsonElement idElement)
                        || !ulong.TryParse(idElement.ToString(), out ulong id))
                        break;
                    await _peerConnectedEvent.InvokeAsync(id).ConfigureAwait(false);
                    await _audioLogger.DebugAsync("Received NewPeer").ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.PeerClosed:
                {
                    if (payload is not JsonElement jsonElement)
                        break;
                    if (!jsonElement.TryGetProperty("peerId", out JsonElement idElement)
                        || !ulong.TryParse(idElement.ToString(), out ulong id))
                        break;
                    if (!jsonElement.TryGetProperty("fromId", out JsonElement fromIdElement)
                        || !ulong.TryParse(fromIdElement.ToString(), out ulong fromId)
                        || fromId != ChannelId)
                        break;

                    await _peerDisconnectedEvent.InvokeAsync(id).ConfigureAwait(false);
                    await _audioLogger.DebugAsync("Received PeerClosed").ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.ResumeHeadset:
                case VoiceSocketFrameType.PauseHeadset:
                case VoiceSocketFrameType.ConsumerResumed:
                case VoiceSocketFrameType.ConsumerPaused:
                case VoiceSocketFrameType.PeerPermissionChanged:
                {
                    await _audioLogger.DebugAsync(type.ToString()).ConfigureAwait(false);
                }
                    break;
                case VoiceSocketFrameType.Disconnect:
                {
                    await Guild.DisconnectAudioAsync().ConfigureAwait(false);
                    await _clientDisconnectedEvent.InvokeAsync().ConfigureAwait(false);
                    await _audioLogger.DebugAsync("Received Disconnect").ConfigureAwait(false);
                }
                    break;
                default:
                    await _audioLogger.WarningAsync($"Unknown VoiceSocketFrameType ({type})").ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _audioLogger.ErrorAsync($"Error Handling {type}: {payload}", ex)
                .ConfigureAwait(false);
        }
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
        try
        {
            await _audioLogger.DebugAsync("RTCP Started").ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                if (LastRtpTimestamp == 0 || SentPackets == 0 || SentOctets == 0)
                    continue;

                _lastRtcpTime = Environment.TickCount;

                try
                {
                    await ApiClient.SendRtcpAsync(_ssrc, LastRtpTimestamp, SentPackets, SentOctets).ConfigureAwait(false);
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
