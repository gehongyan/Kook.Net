using System.Text.Json;
using Kook.Logging;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     KOOK 语音信令客户端：在 <see cref="ProtooPeer"/> 之上提供带类型的请求、服务端请求与通知。
/// </summary>
/// <remarks>
///     本类型只覆盖信令，不涉及 WebRTC 媒体。<c>newConsumer</c> 的应答由上层通过
///     <see cref="NewConsumerHandler"/> 决定：媒体层完成 <c>consume</c> 后再接受，失败则拒绝。
/// </remarks>
internal sealed class VoiceSignalingClient : IDisposable
{
    private readonly ProtooPeer _peer;
    private readonly Logger? _logger;

    public VoiceSignalingClient(ProtooPeer peer, Logger? logger = null)
    {
        _peer = peer;
        _logger = logger;
        _peer.RequestHandler = HandleRequestAsync;
        _peer.NotificationReceived += OnNotificationReceivedAsync;
        _peer.Closed += OnPeerClosedAsync;
    }

    public static VoiceSignalingClient Create(Logger? logger = null) =>
        new(new ProtooPeer(new ClientWebSocketProtooTransport(), logger), logger);

    /// <summary>
    ///     处理 <c>newConsumer</c> 请求。返回 <see cref="ProtooRequestResult.Accept"/> 表示媒体层已接受该 Consumer。
    ///     未设置时，该请求会被拒绝。
    /// </summary>
    public Func<NewConsumerData, Task<ProtooRequestResult>>? NewConsumerHandler { get; set; }

    /// <summary>
    ///     所有服务端通知的原始形式，包括下列已分类的通知。
    /// </summary>
    public event Func<ProtooNotification, Task>? NotificationReceived;

    #region 已分类的通知

    // 载荷无法解析的通知不会引发下列事件，但仍然会通过 NotificationReceived 以原始形式到达。

    /// <summary> <c>newPeer</c>：有成员加入语音频道。 </summary>
    public event Func<VoicePeerInfo, Task>? NewPeer;

    /// <summary> <c>peerClosed</c>：有成员离开语音频道。主动离开与被管理员移出的通知完全相同。 </summary>
    public event Func<VoicePeerRefData, Task>? PeerClosed;

    /// <summary> <c>consumerClosed</c>：一个 Consumer 被关闭。有 Consumer 的成员离开时，先于 <c>peerClosed</c> 到达。 </summary>
    public event Func<VoiceConsumerRefData, Task>? ConsumerClosed;

    /// <summary> <c>consumerPaused</c>：成员的麦克风静音。载荷中没有 <c>consumerId</c>，只有 <c>peerId</c>。 </summary>
    public event Func<VoicePeerRefData, Task>? ConsumerPaused;

    /// <summary> <c>consumerResumed</c>：成员的麦克风取消静音。 </summary>
    public event Func<VoicePeerRefData, Task>? ConsumerResumed;

    /// <summary> <c>pauseHeadset</c>：成员的耳机静音。 </summary>
    public event Func<VoicePeerRefData, Task>? PauseHeadset;

    /// <summary> <c>resumeHeadset</c>：成员的耳机取消静音。 </summary>
    public event Func<VoicePeerRefData, Task>? ResumeHeadset;

    /// <summary> <c>peerPermissionChanged</c>：成员的语音权限或服务端禁麦、禁听状态变化，成员可以是 Bot 自己。 </summary>
    public event Func<VoicePeerPermissionChangedData, Task>? PeerPermissionChanged;

    /// <summary> <c>atmosphere</c>：成员使用了氛围道具。 </summary>
    public event Func<VoiceAtmosphereData, Task>? Atmosphere;

    /// <summary> <c>startAccompaniment</c>：成员开始播放伴奏。 </summary>
    public event Func<VoiceAccompanimentData, Task>? StartAccompaniment;

    /// <summary> <c>stopAccompaniment</c>：成员停止播放伴奏。该通知在实测中尚未出现。 </summary>
    public event Func<VoicePeerRefData, Task>? StopAccompaniment;

    #endregion

    /// <summary>
    ///     服务端发来 <c>active-speaker</c> 请求。载荷在实测中尚未出现，官方 SDK 也不解析它而是原样透传，
    ///     因此这里同样只给出原始的 JSON。
    /// </summary>
    public event Func<JsonElement?, Task>? ActiveSpeaker;

    /// <summary>
    ///     服务端发来 <c>disconnect</c> 通知，例如被管理员移出语音频道（实测 <c>errorCode</c> 为 41106）。
    /// </summary>
    public event Func<VoiceDisconnectData, Task>? Disconnected;

    /// <summary>
    ///     服务端发来 <c>closeByOtherDevice</c> 请求。该请求在实测中尚未出现。
    /// </summary>
    public event Func<Task>? CloseByOtherDevice;

    /// <summary>
    ///     服务端发来 <c>reload</c> 请求，要求客户端重新走 <c>quickJoin</c> 流程。该请求在实测中尚未出现。
    /// </summary>
    public event Func<Task>? ReloadRequested;

    /// <summary>
    ///     信令连接已关闭，包含 WebSocket 关闭码与原因。
    /// </summary>
    public event Func<ProtooCloseInfo, Task>? Closed;

    public bool IsClosed => _peer.IsClosed;

    public Task ConnectAsync(VoiceGatewayToken token, CancellationToken cancellationToken = default)
    {
        Preconditions.NotNull(token, nameof(token));
        return _peer.ConnectAsync(new Uri(token.GatewayUrl), cancellationToken);
    }

    public Task<QuickJoinResponse> QuickJoinAsync(QuickJoinParams args, CancellationToken cancellationToken = default)
    {
        Preconditions.NotNull(args, nameof(args));
        return _peer.RequestAsync<QuickJoinResponse>(VoiceSignalingMethods.QuickJoin, args, cancellationToken: cancellationToken);
    }

    public Task<TurnTransportResponse> CreateTurnTransportAsync(CancellationToken cancellationToken = default) =>
        _peer.RequestAsync<TurnTransportResponse>(VoiceSignalingMethods.CreateTurnTransport, cancellationToken: cancellationToken);

    public Task<InitSpeakingResponse> InitSpeakingAsync(InitSpeakingParams args, CancellationToken cancellationToken = default)
    {
        Preconditions.NotNull(args, nameof(args));
        return _peer.RequestAsync<InitSpeakingResponse>(VoiceSignalingMethods.InitSpeaking, args, cancellationToken: cancellationToken);
    }

    public Task PauseProducerAsync(string producerId, CancellationToken cancellationToken = default) =>
        SendProducerRequestAsync(VoiceSignalingMethods.PauseProducer, producerId, cancellationToken);

    public Task ResumeProducerAsync(string producerId, CancellationToken cancellationToken = default) =>
        SendProducerRequestAsync(VoiceSignalingMethods.ResumeProducer, producerId, cancellationToken);

    public Task CloseProducerAsync(string producerId, CancellationToken cancellationToken = default) =>
        SendProducerRequestAsync(VoiceSignalingMethods.CloseProducer, producerId, cancellationToken);

    public Task CloseAsync(CancellationToken cancellationToken = default) => _peer.CloseAsync(cancellationToken);

    private Task SendProducerRequestAsync(string method, string producerId, CancellationToken cancellationToken)
    {
        Preconditions.NotNullOrWhitespace(producerId, nameof(producerId));
        return _peer.RequestAsync(method, new ProducerParams { ProducerId = producerId }, cancellationToken: cancellationToken);
    }

    private async Task<ProtooRequestResult> HandleRequestAsync(ProtooRequest request)
    {
        switch (request.Method)
        {
            case VoiceSignalingMethods.NewConsumer:
                Func<NewConsumerData, Task<ProtooRequestResult>>? handler = NewConsumerHandler;
                if (handler is null)
                    return ProtooRequestResult.Reject(503, "no consumer handler");
                NewConsumerData? data = request.Data?.Deserialize<NewConsumerData>(ProtooJson.Options);
                return data is null
                    ? ProtooRequestResult.Reject(400, "invalid newConsumer data")
                    : await handler(data).ConfigureAwait(false);
            case VoiceSignalingMethods.ActiveSpeaker:
                Func<JsonElement?, Task>? activeSpeaker = ActiveSpeaker;
                if (activeSpeaker is not null)
                    await activeSpeaker(request.Data).ConfigureAwait(false);
                return ProtooRequestResult.Accept();
            case VoiceSignalingMethods.CloseByOtherDevice:
                await RaiseAsync(CloseByOtherDevice).ConfigureAwait(false);
                return ProtooRequestResult.Accept();
            case VoiceSignalingMethods.Reload:
                // 与 rtc-core 一致：先应答，再由上层重新入会
                _ = Task.Run(() => RaiseAsync(ReloadRequested));
                return ProtooRequestResult.Accept();
            default:
                return ProtooRequestResult.Reject(404, "unknown method");
        }
    }

    private async Task OnNotificationReceivedAsync(ProtooNotification notification)
    {
        switch (notification.Method)
        {
            case VoiceSignalingMethods.Disconnect:
                // 即使没有载荷也要通知上层：服务端已经断开了
                VoiceDisconnectData data = Deserialize<VoiceDisconnectData>(notification) ?? new VoiceDisconnectData();
                Func<VoiceDisconnectData, Task>? disconnected = Disconnected;
                if (disconnected is not null)
                    await disconnected(data).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.NewPeer:
                await RaiseAsync(NewPeer, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.PeerClosed:
                await RaiseAsync(PeerClosed, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.ConsumerClosed:
                await RaiseAsync(ConsumerClosed, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.ConsumerPaused:
                await RaiseAsync(ConsumerPaused, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.ConsumerResumed:
                await RaiseAsync(ConsumerResumed, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.PauseHeadset:
                await RaiseAsync(PauseHeadset, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.ResumeHeadset:
                await RaiseAsync(ResumeHeadset, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.PeerPermissionChanged:
                await RaiseAsync(PeerPermissionChanged, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.Atmosphere:
                await RaiseAsync(Atmosphere, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.StartAccompaniment:
                await RaiseAsync(StartAccompaniment, notification).ConfigureAwait(false);
                break;
            case VoiceSignalingMethods.StopAccompaniment:
                await RaiseAsync(StopAccompaniment, notification).ConfigureAwait(false);
                break;
        }

        Func<ProtooNotification, Task>? handler = NotificationReceived;
        if (handler is not null)
            await handler(notification).ConfigureAwait(false);
    }

    /// <summary>
    ///     解析通知的载荷并引发事件。载荷缺失或无法解析时不引发事件，但会输出警告日志，通知仍会通过 <see cref="NotificationReceived"/> 到达。
    /// </summary>
    private async Task RaiseAsync<T>(Func<T, Task>? handler, ProtooNotification notification)
        where T : class
    {
        if (handler is null)
            return;
        if (Deserialize<T>(notification) is { } data)
            await handler(data).ConfigureAwait(false);
    }

    private T? Deserialize<T>(ProtooNotification notification)
        where T : class
    {
        if (notification.Data is not { } element)
        {
            ReportMalformed(notification, "The notification has no data", null);
            return null;
        }

        try
        {
            T? data = element.Deserialize<T>(ProtooJson.Options);
            if (data is null)
                ReportMalformed(notification, "The notification data is null", null);
            return data;
        }
        catch (JsonException e)
        {
            ReportMalformed(notification, "The notification data does not match the expected shape", e);
            return null;
        }
    }

    private void ReportMalformed(ProtooNotification notification, string reason, Exception? exception)
    {
        string data = notification.Data?.GetRawText() ?? "(none)";
        KookDebugger.DebugAudio($"[Signaling] Failed to parse a notification [Method] {notification.Method} [Reason] {reason} [Data] {data}");
        _ = _logger?.WarningAsync($"Failed to parse the {notification.Method} notification: {reason}. Data: {data}", exception);
    }

    private Task OnPeerClosedAsync(ProtooCloseInfo info) => Closed?.Invoke(info) ?? Task.CompletedTask;

    private static Task RaiseAsync(Func<Task>? handler) => handler?.Invoke() ?? Task.CompletedTask;

    public void Dispose()
    {
        _peer.NotificationReceived -= OnNotificationReceivedAsync;
        _peer.Closed -= OnPeerClosedAsync;
        _peer.RequestHandler = null;
        _peer.Dispose();
    }
}
