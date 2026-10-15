using System.Collections.Concurrent;
using System.Text.Json;
using Kook.API.Rest;
using Kook.Audio.Streams;
using Kook.Audio.WebRtc.Media;
using Kook.Audio.WebRtc.Signaling;
using Kook.Audio.WebRtc.Streams;
using Kook.Logging;
using Kook.Net;
using Kook.Rest.Extensions;
using Kook.WebSocket;
using SIPSorcery.Net;

namespace Kook.Audio.WebRtc;

/// <summary>
///     表示一个基于 WebRTC 的实验性音频客户端。
/// </summary>
/// <remarks>
///     结构与 UDP 版本的 <c>AudioClient</c> 保持一致：由 <see cref="ConnectionManager"/> 管理连接的生命周期，
///     发送链为 <see cref="WebRtcOpusEncodeStream"/>、<see cref="WebRtcBufferedWriteStream"/>、<see cref="WebRtcOutputStream"/>，
///     接收链为 <see cref="WebRtcOpusDecodeStream"/>、<see cref="InputStream"/>。Opus 编解码使用纯托管的 Concentus。
/// </remarks>
internal sealed partial class WebRtcAudioClient : IWebRtcAudioClient
{
    private const int ConnectionTimeoutMs = 30000; // 30 seconds
    private const int MediaConnectTimeoutMs = 15000; // 15 seconds
    private const int DefaultBitrate = 32 * 1000;

    // 发送与接收方向各自的 Opus 负载类型，取自实测：上行由客户端协商为 111，服务端下发的 Consumer 为 100
    private const int SendPayloadType = 111;
    private const int ReceivePayloadType = 100;
    private const string OpusFmtp = "minptime=10;useinbandfec=1";

    private readonly Logger _audioLogger;
    private readonly SemaphoreSlim _stateLock;
    private readonly ConcurrentDictionary<uint, StreamPair> _streams;
    private readonly ConcurrentDictionary<uint, ulong> _streamUsers;
    private readonly ConcurrentDictionary<string, uint> _consumerSsrcs;

    private VoiceSignalingClient? _signaling;
    private WebRtcMediaTransport? _sendTransport;
    private WebRtcMediaTransport? _receiveTransport;
    private string? _producerId;
    private int _bitrate = DefaultBitrate;
    private volatile bool _isMicrophoneMuted = true;

    internal ConnectionManager Connection { get; }

    internal bool IsFinished { get; private set; }

    public SocketVoiceChannel Channel { get; }

    public ulong ChannelId => Channel.Id;

    public SocketGuild Guild => Channel.Guild;

    private KookSocketClient Kook => Guild.Kook;

    public ConnectionState ConnectionState => Connection.State;

    /// <inheritdoc />
    /// <remarks> WebRTC 传输不经过官方的 UDP 语音服务器，此值恒为 <c>0</c>。 </remarks>
    public int UdpLatency => 0;

    public bool IsMicrophoneMuted => _isMicrophoneMuted;

    /// <summary>
    ///     初始化一个 <see cref="WebRtcAudioClient"/> 类的新实例。
    /// </summary>
    internal WebRtcAudioClient(SocketVoiceChannel voiceChannel, int clientId)
    {
        Channel = voiceChannel;
        _audioLogger = Kook.LogManager.CreateLogger($"WebRTC Audio #{clientId}");

        _stateLock = new SemaphoreSlim(1, 1);
        Connection = new ConnectionManager(_stateLock, _audioLogger, ConnectionTimeoutMs,
            OnConnectingAsync, OnDisconnectingAsync, x => _signalingClosedEvent.Add(x));
        Connection.Connected += () => _connectedEvent.InvokeAsync();
        Connection.Disconnected += (ex, _) => _disconnectedEvent.InvokeAsync(ex);

        _streams = new ConcurrentDictionary<uint, StreamPair>();
        _streamUsers = new ConcurrentDictionary<uint, ulong>();
        _consumerSsrcs = new ConcurrentDictionary<string, uint>();
    }

    internal Task StartAsync() => Connection.StartAsync();

    public Task StopAsync() => Connection.StopAsync();

    public IReadOnlyDictionary<uint, AudioInStream> GetStreams() =>
        _streams.ToDictionary(pair => pair.Key, pair => pair.Value.Reader);

    public bool TryGetStreamUserId(uint ssrc, out ulong userId) => _streamUsers.TryGetValue(ssrc, out userId);

    public async Task SetMicrophoneMutedAsync(bool muted)
    {
        VoiceSignalingClient signaling = _signaling
            ?? throw new InvalidOperationException("The audio client is not connected.");
        string producerId = _producerId
            ?? throw new InvalidOperationException("The audio client has no producer.");
        if (muted == _isMicrophoneMuted)
            return;

        if (muted)
            await signaling.PauseProducerAsync(producerId, Connection.CancellationToken).ConfigureAwait(false);
        else
            await signaling.ResumeProducerAsync(producerId, Connection.CancellationToken).ConfigureAwait(false);
        _isMicrophoneMuted = muted;
        KookDebugger.DebugAudio($"[Audio] Changed the microphone state [Muted] {muted} [Producer ID] {producerId}");
    }

    private async Task OnConnectingAsync()
    {
        CancellationToken cancellationToken = Connection.CancellationToken;

        await _audioLogger.DebugAsync("Requesting the voice gateway token").ConfigureAwait(false);
        GetVoiceGatewayResponse gateway = await Kook.ApiClient.GetVoiceGatewayAsync(ChannelId).ConfigureAwait(false);
        VoiceGatewayToken token = VoiceGatewayToken.Create(gateway);
        KookDebugger.DebugAudio($"[Audio] Got the voice gateway token [Gateway] {token.RedactedGatewayUrl} "
            + $"[PC Voice SDK] {token.PcVoiceSdk} [iOS Voice SDK] {token.IosVoiceSdk}");

        await _audioLogger.DebugAsync("Connecting the signaling client").ConfigureAwait(false);
        VoiceSignalingClient signaling = VoiceSignalingClient.Create(_audioLogger);
        _signaling = signaling;
        signaling.NewConsumerHandler = OnNewConsumerAsync;
        SubscribeSignaling(signaling);
        await signaling.ConnectAsync(token, cancellationToken).ConfigureAwait(false);

        // 实测：不要求中继（forceRelay:false）时，客户端可直接通过 UDP 连通服务端的公网候选，无需 TURN
        QuickJoinResponse join = await signaling
            .QuickJoinAsync(new QuickJoinParams { ForceRelay = false }, cancellationToken).ConfigureAwait(false);
        _bitrate = join.RtpConfig?.Audio?.MaxAverageBitrate ?? DefaultBitrate;

        WebRtcMediaTransport sendTransport = new("send", WebRtcMediaDirection.Send, SendPayloadType, OpusFmtp, _audioLogger);
        WebRtcMediaTransport receiveTransport = new("receive", WebRtcMediaDirection.Receive, ReceivePayloadType, OpusFmtp, _audioLogger);
        _sendTransport = sendTransport;
        _receiveTransport = receiveTransport;
        receiveTransport.RtpPacketReceived += OnRtpPacketReceived;
        sendTransport.ConnectionStateChanged += OnTransportStateChanged;
        receiveTransport.ConnectionStateChanged += OnTransportStateChanged;

        WebRtcLocalDescription sendLocal = await sendTransport.CreateLocalDescriptionAsync().ConfigureAwait(false);
        WebRtcLocalDescription receiveLocal = await receiveTransport.CreateLocalDescriptionAsync().ConfigureAwait(false);

        // 先把本端的 DTLS 指纹告知服务端，再开始 ICE 与 DTLS，这样服务端能在握手时校验客户端证书
        InitSpeakingResponse speaking = await signaling
            .InitSpeakingAsync(CreateInitSpeakingParams(sendLocal, receiveLocal), cancellationToken).ConfigureAwait(false);
        _producerId = speaking.Producer?.Id
            ?? throw new InvalidOperationException("The server did not return a producer for the audio client.");
        _isMicrophoneMuted = true;
        KookDebugger.DebugAudio($"[Audio] Joined the channel [Channel ID] {ChannelId} [Producer ID] {_producerId} "
            + $"[Peers] {speaking.Peers?.Count ?? 0} [Bitrate] {_bitrate}");

        sendTransport.ApplyRemoteParameters(join.ProducerTransport);
        receiveTransport.ApplyRemoteParameters(join.ConsumerTransport);
        TimeSpan mediaTimeout = TimeSpan.FromMilliseconds(MediaConnectTimeoutMs);
        await Task.WhenAll(
            sendTransport.WaitConnectedAsync(mediaTimeout, cancellationToken),
            receiveTransport.WaitConnectedAsync(mediaTimeout, cancellationToken)).ConfigureAwait(false);

        await Connection.CompleteAsync().ConfigureAwait(false);
    }

    private static InitSpeakingParams CreateInitSpeakingParams(WebRtcLocalDescription send, WebRtcLocalDescription receive)
    {
        var rtpParameters = new
        {
            mid = send.Mid,
            codecs = new[]
            {
                new
                {
                    mimeType = "audio/opus",
                    payloadType = SendPayloadType,
                    clockRate = WebRtcAudioFormat.SamplingRate,
                    channels = WebRtcAudioFormat.Channels,
                    parameters = new Dictionary<string, object>
                    {
                        ["minptime"] = 10,
                        ["useinbandfec"] = 1,
                        ["sprop-stereo"] = 1,
                        ["ptime"] = WebRtcAudioFormat.FrameMillis
                    },
                    rtcpFeedback = new[] { new { type = "nack", parameter = string.Empty } }
                }
            },
            headerExtensions = Array.Empty<object>(),
            encodings = new[] { new { ssrc = send.Ssrc, dtx = false } },
            rtcp = new { cname = send.Cname, reducedSize = true }
        };

        return new InitSpeakingParams
        {
            PauseHeadSet = false,
            PauseMic = true,
            StartAiReduce = false,
            StartAccompaniment = false,
            RtpParameters = JsonSerializer.SerializeToElement(rtpParameters, ProtooJson.Options),
            ProducerDtls = new VoiceDtlsParameters
            {
                Role = "client",
                Fingerprints = [new VoiceDtlsFingerprint { Algorithm = send.FingerprintAlgorithm, Value = send.Fingerprint }]
            },
            ConsumerDtls = new VoiceDtlsParameters
            {
                Role = "client",
                Fingerprints = [new VoiceDtlsFingerprint { Algorithm = receive.FingerprintAlgorithm, Value = receive.Fingerprint }]
            }
        };
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _audioLogger.DebugAsync("Disconnecting the WebRTC transports").ConfigureAwait(false);

        VoiceSignalingClient? signaling = _signaling;
        _signaling = null;
        if (signaling is not null)
        {
            signaling.NewConsumerHandler = null;
            UnsubscribeSignaling(signaling);
            try
            {
                await signaling.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception closeException)
            {
                await _audioLogger.DebugAsync("Failed to close the signaling connection gracefully", closeException).ConfigureAwait(false);
            }
            signaling.Dispose();
        }

        DisposeTransport(ref _sendTransport);
        DisposeTransport(ref _receiveTransport);
        _producerId = null;
        _isMicrophoneMuted = true;

        await _audioLogger.DebugAsync("Finishing the audio connection").ConfigureAwait(false);
        await Connection.StopAsync().ConfigureAwait(false);
        await ClearInputStreamsAsync().ConfigureAwait(false);
        IsFinished = true;
    }

    private void DisposeTransport(ref WebRtcMediaTransport? transport)
    {
        WebRtcMediaTransport? current = transport;
        transport = null;
        if (current is null)
            return;
        current.RtpPacketReceived -= OnRtpPacketReceived;
        current.ConnectionStateChanged -= OnTransportStateChanged;
        current.Dispose();
    }

    #region Signaling

    private async Task<ProtooRequestResult> OnNewConsumerAsync(NewConsumerData data)
    {
        if (!TryReadSsrc(data.RtpParameters, out uint ssrc))
            return ProtooRequestResult.Reject(400, "missing ssrc");

        ulong.TryParse(data.PeerId, out ulong userId);
        if (_streams.ContainsKey(ssrc))
            return ProtooRequestResult.Accept();

        InputStream readerStream = new();                          // Consumes header
        WebRtcOpusDecodeStream opusDecoder = new(readerStream);    // Passes header
        // 先登记 SSRC 所属的用户，再登记音频流：只要能按 SSRC 找到音频流，就一定能找到它所属的用户
        _streamUsers[ssrc] = userId;
        _streams[ssrc] = new StreamPair(readerStream, opusDecoder);
        _consumerSsrcs[data.Id] = ssrc;
        KookDebugger.DebugAudio($"[Audio] Created an input stream [SSRC] {ssrc} [User ID] {data.PeerId} "
            + $"[Consumer ID] {data.Id} [Producer ID] {data.ProducerId}");
        await _streamCreatedEvent.InvokeAsync(ssrc, readerStream).ConfigureAwait(false);
        return ProtooRequestResult.Accept();
    }

    private static bool TryReadSsrc(JsonElement rtpParameters, out uint ssrc)
    {
        ssrc = 0;
        return rtpParameters.ValueKind == JsonValueKind.Object
            && rtpParameters.TryGetProperty("encodings", out JsonElement encodings)
            && encodings.ValueKind == JsonValueKind.Array
            && encodings.GetArrayLength() > 0
            && encodings[0].TryGetProperty("ssrc", out JsonElement value)
            && value.TryGetUInt32(out ssrc);
    }

    private void SubscribeSignaling(VoiceSignalingClient signaling)
    {
        signaling.NewPeer += OnNewPeerAsync;
        signaling.PeerClosed += OnPeerClosedAsync;
        signaling.ConsumerClosed += OnConsumerClosedAsync;
        signaling.ConsumerPaused += OnConsumerPausedAsync;
        signaling.ConsumerResumed += OnConsumerResumedAsync;
        signaling.PauseHeadset += OnPauseHeadsetAsync;
        signaling.ResumeHeadset += OnResumeHeadsetAsync;
        signaling.PeerPermissionChanged += OnPeerPermissionChangedAsync;
        signaling.Atmosphere += OnAtmosphereAsync;
        signaling.StartAccompaniment += OnStartAccompanimentAsync;
        signaling.StopAccompaniment += OnStopAccompanimentAsync;
        signaling.Disconnected += OnSignalingDisconnectedAsync;
        signaling.CloseByOtherDevice += OnCloseByOtherDeviceAsync;
        signaling.ReloadRequested += OnReloadRequestedAsync;
        signaling.Closed += OnSignalingClosedAsync;
    }

    private void UnsubscribeSignaling(VoiceSignalingClient signaling)
    {
        signaling.NewPeer -= OnNewPeerAsync;
        signaling.PeerClosed -= OnPeerClosedAsync;
        signaling.ConsumerClosed -= OnConsumerClosedAsync;
        signaling.ConsumerPaused -= OnConsumerPausedAsync;
        signaling.ConsumerResumed -= OnConsumerResumedAsync;
        signaling.PauseHeadset -= OnPauseHeadsetAsync;
        signaling.ResumeHeadset -= OnResumeHeadsetAsync;
        signaling.PeerPermissionChanged -= OnPeerPermissionChangedAsync;
        signaling.Atmosphere -= OnAtmosphereAsync;
        signaling.StartAccompaniment -= OnStartAccompanimentAsync;
        signaling.StopAccompaniment -= OnStopAccompanimentAsync;
        signaling.Disconnected -= OnSignalingDisconnectedAsync;
        signaling.CloseByOtherDevice -= OnCloseByOtherDeviceAsync;
        signaling.ReloadRequested -= OnReloadRequestedAsync;
        signaling.Closed -= OnSignalingClosedAsync;
    }

    private Task OnNewPeerAsync(VoicePeerInfo info)
    {
        if (VoicePeer.Create(info) is { } peer)
            return _peerJoinedEvent.InvokeAsync(peer);
        ReportInvalidPeerId(VoiceSignalingMethods.NewPeer, info.PeerId ?? info.Id);
        return Task.CompletedTask;
    }

    private async Task OnPeerClosedAsync(VoicePeerRefData data)
    {
        if (!TryParseUserId(VoiceSignalingMethods.PeerClosed, data.PeerId, out ulong userId))
            return;

        // 成员离开时，他的所有音频流都随之销毁，StreamDestroyed 先于 PeerLeft 引发
        foreach (uint ssrc in _streamUsers.Where(x => x.Value == userId).Select(x => x.Key).ToArray())
            await DestroyInputStreamAsync(ssrc).ConfigureAwait(false);
        await _peerLeftEvent.InvokeAsync(userId).ConfigureAwait(false);
    }

    private async Task OnConsumerClosedAsync(VoiceConsumerRefData data)
    {
        if (data.ConsumerId is not { Length: > 0 } consumerId)
        {
            ReportInvalidPeerId(VoiceSignalingMethods.ConsumerClosed, null, "consumerId");
            return;
        }

        if (_consumerSsrcs.TryRemove(consumerId, out uint ssrc))
            await DestroyInputStreamAsync(ssrc).ConfigureAwait(false);
        else
            KookDebugger.DebugAudio($"[Audio] Ignored a consumerClosed notification of an unknown consumer [Consumer ID] {consumerId}");
    }

    // consumerPaused / consumerResumed 的载荷里没有 consumerId，只有 peerId，指的是该成员的麦克风
    private Task OnConsumerPausedAsync(VoicePeerRefData data) =>
        TryParseUserId(VoiceSignalingMethods.ConsumerPaused, data.PeerId, out ulong userId)
            ? _peerMicrophoneMuteChangedEvent.InvokeAsync(userId, true)
            : Task.CompletedTask;

    private Task OnConsumerResumedAsync(VoicePeerRefData data) =>
        TryParseUserId(VoiceSignalingMethods.ConsumerResumed, data.PeerId, out ulong userId)
            ? _peerMicrophoneMuteChangedEvent.InvokeAsync(userId, false)
            : Task.CompletedTask;

    private Task OnPauseHeadsetAsync(VoicePeerRefData data) =>
        TryParseUserId(VoiceSignalingMethods.PauseHeadset, data.PeerId, out ulong userId)
            ? _peerHeadsetMuteChangedEvent.InvokeAsync(userId, true)
            : Task.CompletedTask;

    private Task OnResumeHeadsetAsync(VoicePeerRefData data) =>
        TryParseUserId(VoiceSignalingMethods.ResumeHeadset, data.PeerId, out ulong userId)
            ? _peerHeadsetMuteChangedEvent.InvokeAsync(userId, false)
            : Task.CompletedTask;

    private Task OnPeerPermissionChangedAsync(VoicePeerPermissionChangedData data) =>
        TryParseUserId(VoiceSignalingMethods.PeerPermissionChanged, data.PeerId, out ulong userId)
            ? _peerPermissionChangedEvent.InvokeAsync(userId, new VoicePeerPermissions(data.IsServerMute,
                data.IsServerMuteHeadset, data.PermissionFreeMic, data.PermissionLink, data.PermissionSay,
                data.PermissionVoiceManage))
            : Task.CompletedTask;

    private Task OnAtmosphereAsync(VoiceAtmosphereData data) =>
        TryParseUserId(VoiceSignalingMethods.Atmosphere, data.PeerId, out ulong userId)
            ? _atmosphereReceivedEvent.InvokeAsync(userId, data.ItemId)
            : Task.CompletedTask;

    private Task OnStartAccompanimentAsync(VoiceAccompanimentData data) =>
        TryParseUserId(VoiceSignalingMethods.StartAccompaniment, data.PeerId, out ulong userId)
            ? _accompanimentStartedEvent.InvokeAsync(userId,
                new VoiceAccompaniment(data.Data?.MusicName, data.Data?.MusicSoftware, data.Data?.Singer))
            : Task.CompletedTask;

    private Task OnStopAccompanimentAsync(VoicePeerRefData data) =>
        TryParseUserId(VoiceSignalingMethods.StopAccompaniment, data.PeerId, out ulong userId)
            ? _accompanimentStoppedEvent.InvokeAsync(userId)
            : Task.CompletedTask;

    /// <summary>
    ///     解析通知中的成员 ID。无法解析时输出警告日志，调用方不应当引发事件。
    /// </summary>
    private bool TryParseUserId(string method, string? peerId, out ulong userId)
    {
        userId = 0;
        if (peerId is not null && ulong.TryParse(peerId, out userId))
            return true;
        ReportInvalidPeerId(method, peerId);
        return false;
    }

    private void ReportInvalidPeerId(string method, string? value, string field = "peerId")
    {
        KookDebugger.DebugAudio($"[Audio] Dropped a notification with an invalid field [Method] {method} [Field] {field} [Value] {value ?? "(null)"}");
        _ = _audioLogger.WarningAsync($"Dropped the {method} notification because its {field} is missing or invalid: {value ?? "(null)"}");
    }

    private async Task OnCloseByOtherDeviceAsync()
    {
        KookDebugger.DebugAudio("[Audio] Closed by another device");
        await _closedByOtherDeviceEvent.InvokeAsync().ConfigureAwait(false);
        await _signalingClosedEvent.InvokeAsync(
            new InvalidOperationException("The server closed the audio client because the user joined from another device.")).ConfigureAwait(false);
    }

    private async Task OnReloadRequestedAsync()
    {
        KookDebugger.DebugAudio("[Audio] The server requested a reload");
        await _reloadRequestedEvent.InvokeAsync().ConfigureAwait(false);
        // 重新加入需要重建全部媒体资源，这个客户端不支持，只能断开，由调用方重新连接
        await _signalingClosedEvent.InvokeAsync(
            new InvalidOperationException("The server requested the audio client to reload, which is not supported.")).ConfigureAwait(false);
    }

    private async Task OnSignalingDisconnectedAsync(VoiceDisconnectData data)
    {
        KookDebugger.DebugAudio($"[Audio] Disconnected by the server [Error Code] {data.ErrorCode} [Error Reason] {data.ErrorReason}");
        await _serverDisconnectedEvent.InvokeAsync(data.ErrorCode, data.ErrorReason).ConfigureAwait(false);
        await _signalingClosedEvent.InvokeAsync(
            new InvalidOperationException($"The server disconnected the audio client (code {data.ErrorCode}): {data.ErrorReason}")).ConfigureAwait(false);
    }

    private Task OnSignalingClosedAsync(ProtooCloseInfo info)
    {
        KookDebugger.DebugAudio($"[Audio] The signaling connection was closed [Code] {info.Code} [Reason] {info.Reason}");
        return _signalingClosedEvent.InvokeAsync(info.Error ?? new WebSocketClosedException(info.Code, info.Reason));
    }

    private void OnTransportStateChanged(RTCPeerConnectionState state)
    {
        if (state is RTCPeerConnectionState.failed or RTCPeerConnectionState.closed)
        {
            _ = _signalingClosedEvent.InvokeAsync(
                new InvalidOperationException($"A WebRTC transport of the audio client is {state}."));
        }
    }

    #endregion

    #region Media

    private void OnRtpPacketReceived(RTPPacket packet)
    {
        uint ssrc = packet.Header.SyncSource;
        if (!_streams.TryGetValue(ssrc, out StreamPair streamPair))
        {
            KookDebugger.DebugAudio($"[Audio] Dropped a packet from an unknown stream [SSRC] {ssrc} [Payload Type] {packet.Header.PayloadType}");
            return;
        }

        try
        {
            byte[] payload = packet.GetPayloadBytes();
            // 输入流的写入是同步完成的，在此处同步等待可以保证同一 SSRC 的包按到达顺序依次解码
            streamPair.Writer.WriteHeader(packet.Header.SequenceNumber, packet.Header.Timestamp, false);
            streamPair.Writer.WriteAsync(payload, 0, payload.Length).GetAwaiter().GetResult();
            KookDebugger.DebugAudio($"[Audio] Received a packet [SSRC] {ssrc} [Sequence] {packet.Header.SequenceNumber} "
                + $"[Timestamp] {packet.Header.Timestamp} [Bytes] {payload.Length}");
        }
        catch (Exception ex)
        {
            _ = _audioLogger.WarningAsync("Failed to process an RTP packet", ex);
        }
    }

    internal void SendOpusPacket(byte[] payload)
    {
        WebRtcMediaTransport? transport = _sendTransport;
        if (transport is null || Connection.State != ConnectionState.Connected)
            return;
        if (_isMicrophoneMuted)
        {
            KookDebugger.DebugAudio($"[Audio] Dropped a packet because the microphone is muted [Bytes] {payload.Length}");
            return;
        }

        transport.SendOpus((uint)WebRtcAudioFormat.FrameSamplesPerChannel, payload);
    }

    private async Task DestroyInputStreamAsync(uint ssrc)
    {
        if (!_streams.TryRemove(ssrc, out StreamPair streamPair))
            return;
        _streamUsers.TryRemove(ssrc, out ulong userId);
        KookDebugger.DebugAudio($"[Audio] Destroyed an input stream [SSRC] {ssrc} [User ID] {userId}");
        await _streamDestroyedEvent.InvokeAsync(ssrc).ConfigureAwait(false);
        streamPair.Writer.Dispose();
    }

    private async Task ClearInputStreamsAsync()
    {
        foreach (uint ssrc in _streams.Keys.ToArray())
            await DestroyInputStreamAsync(ssrc).ConfigureAwait(false);
        _streams.Clear();
        _streamUsers.Clear();
        _consumerSsrcs.Clear();
    }

    #endregion

    #region Streams

    /// <inheritdoc />
    public AudioOutStream CreateOpusStream(int bufferMillis = 1000)
    {
        WebRtcOutputStream outputStream = new(this);                                                                // Ignores header
        return new WebRtcBufferedWriteStream(outputStream, bufferMillis, Connection.CancellationToken, _audioLogger); // Ignores header, generates timing
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectOpusStream() => new WebRtcOutputStream(this);

    /// <inheritdoc />
    public AudioOutStream CreatePcmStream(AudioApplication application, int? bitrate = null, int bufferMillis = 1000, int packetLoss = 30)
    {
        WebRtcOutputStream outputStream = new(this);
        WebRtcBufferedWriteStream bufferedStream = new(outputStream, bufferMillis, Connection.CancellationToken, _audioLogger);
        return new WebRtcOpusEncodeStream(bufferedStream, bitrate ?? _bitrate, application, packetLoss);
    }

    /// <inheritdoc />
    public AudioOutStream CreateDirectPcmStream(AudioApplication application, int? bitrate = null, int packetLoss = 30)
    {
        WebRtcOutputStream outputStream = new(this);
        return new WebRtcOpusEncodeStream(outputStream, bitrate ?? _bitrate, application, packetLoss);
    }

    #endregion

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAsync().GetAwaiter().GetResult();
            _stateLock.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose() => Dispose(true);

    private readonly struct StreamPair(AudioInStream reader, AudioOutStream writer)
    {
        public AudioInStream Reader => reader;
        public AudioOutStream Writer => writer;
    }
}
