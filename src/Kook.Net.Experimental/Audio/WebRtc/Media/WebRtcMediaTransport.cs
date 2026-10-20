using System.Text.RegularExpressions;
using Kook.Audio.WebRtc.Signaling;
using Kook.Logging;
using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;

namespace Kook.Audio.WebRtc.Media;

/// <summary>
///     一条 WebRTC 媒体传输（对应服务端的 pTransport 或 cTransport），封装一个 <see cref="RTCPeerConnection"/>。
/// </summary>
/// <remarks>
///     发送传输只发不收，接收传输只收不发。本类型不走信令里的 SDP 协商：本端 offer 只用来取得 DTLS 指纹与 SSRC，
///     远端 answer 由 <see cref="WebRtcSdpBuilder"/> 用传输参数拼出。
/// </remarks>
internal sealed class WebRtcMediaTransport : IDisposable
{
    private readonly RTCPeerConnection _peerConnection;
    private readonly Logger _logger;
    private readonly int _payloadType;
    private readonly string _fmtp;
    private readonly TaskCompletionSource<bool> _connected = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal WebRtcMediaTransport(string name, WebRtcMediaDirection direction, int payloadType, string fmtp, Logger logger)
    {
        Name = name;
        Direction = direction;
        _payloadType = payloadType;
        _fmtp = fmtp;
        _logger = logger;

        _peerConnection = new RTCPeerConnection(new RTCConfiguration());
        AudioFormat format = new(AudioCodecsEnum.OPUS, payloadType, WebRtcAudioFormat.SamplingRate, WebRtcAudioFormat.Channels, fmtp);
        _peerConnection.addTrack(new MediaStreamTrack(format,
            direction == WebRtcMediaDirection.Send ? MediaStreamStatusEnum.SendOnly : MediaStreamStatusEnum.RecvOnly));

        _peerConnection.oniceconnectionstatechange += state =>
        {
            KookDebugger.DebugAudio($"[Ice] State changed [Transport] {Name} [State] {state}");
            LogNominatedPair();
        };
        _peerConnection.onconnectionstatechange += OnConnectionStateChanged;
        _peerConnection.onicecandidate += candidate =>
            KookDebugger.DebugAudio($"[Ice] Gathered a local candidate [Transport] {Name} [Candidate] {candidate?.candidate}");
        _peerConnection.OnRtpClosed += reason =>
            KookDebugger.DebugAudio($"[Transport] RTP session closed [Transport] {Name} [Reason] {reason}");
        _peerConnection.OnTimeout += mediaType =>
            KookDebugger.DebugAudio($"[Transport] Media timed out [Transport] {Name} [Media] {mediaType}");
        _peerConnection.OnRtpPacketReceived += (_, mediaType, packet) =>
        {
            if (mediaType == SDPMediaTypesEnum.audio)
                RtpPacketReceived?.Invoke(packet);
        };
    }

    internal string Name { get; }

    internal WebRtcMediaDirection Direction { get; }

    internal WebRtcLocalDescription? Local { get; private set; }

    internal RTCPeerConnectionState ConnectionState => _peerConnection.connectionState;

    internal event Action<RTPPacket>? RtpPacketReceived;

    internal event Action<RTCPeerConnectionState>? ConnectionStateChanged;

    /// <summary>
    ///     生成并应用本端 offer，返回需要告知服务端的 DTLS 指纹、SSRC 等信息。
    /// </summary>
    internal async Task<WebRtcLocalDescription> CreateLocalDescriptionAsync()
    {
        RTCSessionDescriptionInit offer = _peerConnection.createOffer(null);
        await _peerConnection.setLocalDescription(offer).ConfigureAwait(false);
        string sdp = offer.sdp;

        Match fingerprint = FingerprintLine.Match(sdp);
        Match mid = MidLine.Match(sdp);
        if (!fingerprint.Success || !mid.Success)
            throw new InvalidOperationException("The local SDP offer has no DTLS fingerprint or mid.");

        Match cname = CnameLine.Match(sdp);
        uint ssrc = _peerConnection.AudioStream?.LocalTrack?.Ssrc ?? 0;
        Local = new WebRtcLocalDescription(
            mid.Groups[1].Value,
            ssrc,
            cname.Success ? cname.Groups[1].Value : Guid.NewGuid().ToString("N").Substring(0, 16),
            fingerprint.Groups[1].Value,
            fingerprint.Groups[2].Value);
        KookDebugger.DebugAudio($"[Transport] Created the local description [Transport] {Name} "
            + $"[Mid] {Local.Mid} [SSRC] {Local.Ssrc} [Fingerprint Algorithm] {Local.FingerprintAlgorithm}");
        return Local;
    }

    /// <summary>
    ///     应用服务端给出的传输参数，之后 ICE 连通性检查与 DTLS 握手会自动开始。
    /// </summary>
    internal void ApplyRemoteParameters(VoiceTransportParameters remote)
    {
        WebRtcLocalDescription local = Local
            ?? throw new InvalidOperationException("The local description must be created first.");
        string sdp = WebRtcSdpBuilder.BuildAnswer(local.Mid,
            Direction == WebRtcMediaDirection.Send ? "recvonly" : "sendonly", _payloadType, _fmtp, remote);

        SetDescriptionResultEnum result = _peerConnection.setRemoteDescription(
            new RTCSessionDescriptionInit { type = RTCSdpType.answer, sdp = sdp });
        KookDebugger.DebugAudio($"[Transport] Applied the remote parameters [Transport] {Name} [Result] {result} "
            + $"[Transport ID] {remote.Id} [ICE Lite] {remote.IceParameters.IceLite} [Candidates] {remote.IceCandidates.Count}");
        if (result != SetDescriptionResultEnum.OK)
            throw new InvalidOperationException($"Applying the remote description to the {Name} transport failed: {result}.");
    }

    internal async Task WaitConnectedAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Task completed = await Task.WhenAny(_connected.Task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false);
        if (completed != _connected.Task)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new TimeoutException($"The {Name} transport did not connect within {timeout.TotalSeconds:0} seconds "
                + $"(ICE: {_peerConnection.iceConnectionState}, connection: {ConnectionState}).");
        }
        await _connected.Task.ConfigureAwait(false);
    }

    /// <summary>
    ///     发送一个 Opus 包，RTP 头部（序列号、时间戳、SSRC）由底层生成。
    /// </summary>
    /// <param name="durationRtpUnits"> 此包所含音频的时长，以 RTP 时钟（48 kHz）的刻度表示。 </param>
    /// <param name="payload"> Opus 编码的有效负载。 </param>
    internal void SendOpus(uint durationRtpUnits, byte[] payload) =>
        _peerConnection.SendAudio(durationRtpUnits, payload);

    private void OnConnectionStateChanged(RTCPeerConnectionState state)
    {
        _ = _logger.DebugAsync($"The {Name} transport is {state} (ICE: {_peerConnection.iceConnectionState})");
        KookDebugger.DebugAudio($"[Transport] State changed [Transport] {Name} [State] {state} "
            + $"[ICE] {_peerConnection.iceConnectionState}");
        switch (state)
        {
            case RTCPeerConnectionState.connected:
                _connected.TrySetResult(true);
                break;
            case RTCPeerConnectionState.failed:
            case RTCPeerConnectionState.closed:
                _connected.TrySetException(new InvalidOperationException($"The {Name} transport is {state}."));
                break;
        }
        ConnectionStateChanged?.Invoke(state);
    }

    private void LogNominatedPair()
    {
        try
        {
            if (_peerConnection.GetRtpChannel()?.NominatedEntry is { } entry)
            {
                KookDebugger.DebugAudio($"[Ice] Selected a candidate pair [Transport] {Name} "
                    + $"[Remote Type] {entry.RemoteCandidate.type} [Remote] {entry.RemoteCandidate.DestinationEndPoint} "
                    + $"[Local Type] {entry.LocalCandidate.type}");
            }
        }
        catch (Exception ex)
        {
            KookDebugger.DebugAudio($"[Ice] Failed to read the selected candidate pair [Transport] {Name} [Error] {ex.Message}");
        }
    }

    private static readonly Regex FingerprintLine = new(@"^a=fingerprint:(\S+) (\S+)\s*$", RegexOptions.Multiline);
    private static readonly Regex MidLine = new(@"^a=mid:(\S+)\s*$", RegexOptions.Multiline);
    private static readonly Regex CnameLine = new(@"^a=ssrc:\d+ cname:(\S+)\s*$", RegexOptions.Multiline);

    /// <inheritdoc />
    public void Dispose() => _peerConnection.Close("dispose");
}
