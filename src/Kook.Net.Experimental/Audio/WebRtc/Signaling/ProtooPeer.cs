using System.Collections.Concurrent;
using System.Text.Json;
using Kook.Logging;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     protoo 对等端：请求与响应的关联、超时、服务端请求的分发、通知转发以及文本心跳。
/// </summary>
/// <remarks>
///     <para>
///         响应在接收循环中同步处理；服务端请求在独立任务中处理，以免耗时的请求处理阻塞后续帧的接收；
///         通知在接收循环中按到达顺序依次处理。
///     </para>
///     <para>本类型不负责重连，重连策略由上层决定。</para>
/// </remarks>
internal sealed class ProtooPeer : IDisposable
{
    public const string SubProtocol = "protoo";

    // 来自 rtc-core 的实测行为：连接建立后每 60 秒发送一次纯文本 "ping"
    public static readonly TimeSpan DefaultHeartbeatInterval = TimeSpan.FromSeconds(60);

    private const string HeartbeatText = "ping";

    private readonly IProtooTransport _transport;
    private readonly Logger? _logger;
    private readonly TimeSpan _heartbeatInterval;
    private readonly ConcurrentDictionary<long, TaskCompletionSource<ProtooResponse>> _pending = new();
    private readonly Random _random = new();
    private readonly object _randomLock = new();
    private CancellationTokenSource? _heartbeatCts;
    private int _closed;

    public ProtooPeer(IProtooTransport transport, Logger? logger = null, TimeSpan? heartbeatInterval = null)
    {
        _transport = transport;
        _logger = logger;
        _heartbeatInterval = heartbeatInterval ?? DefaultHeartbeatInterval;
        _transport.TextReceived += OnTextReceivedAsync;
        _transport.Closed += OnTransportClosedAsync;
    }

    /// <summary>
    ///     处理服务端发来的请求。未设置时，所有服务端请求都会被拒绝（404）。
    /// </summary>
    public Func<ProtooRequest, Task<ProtooRequestResult>>? RequestHandler { get; set; }

    public event Func<ProtooNotification, Task>? NotificationReceived;

    public event Func<ProtooCloseInfo, Task>? Closed;

    public bool IsClosed => Volatile.Read(ref _closed) == 1;

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        await _transport.ConnectAsync(uri, SubProtocol, cancellationToken).ConfigureAwait(false);
        _heartbeatCts = new CancellationTokenSource();
        _ = HeartbeatLoopAsync(_heartbeatCts.Token);
    }

    /// <summary>
    ///     发送请求并等待响应，返回响应中的 <c>data</c>（可能为 <c>null</c>）。
    /// </summary>
    /// <exception cref="ProtooException"> 服务端返回失败响应，或连接在等待期间关闭。 </exception>
    /// <exception cref="TimeoutException"> 等待响应超时。 </exception>
    public async Task<JsonElement?> RequestAsync(string method, object? data = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Preconditions.NotNullOrWhitespace(method, nameof(method));
        if (IsClosed)
            throw new ProtooException("The peer is closed.");

        TaskCompletionSource<ProtooResponse> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        long id = RegisterPending(completion);
        try
        {
            await _transport.SendTextAsync(ProtooMessageCodec.EncodeRequest(id, method, data), cancellationToken)
                .ConfigureAwait(false);
            KookDebugger.DebugAudio($"[Signaling] Sent a request [Method] {method} [ID] {id} [Data] {DescribeData(data)}");

            TimeSpan effectiveTimeout = timeout ?? GetDefaultTimeout();
            using CancellationTokenSource delayCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task completed = await Task.WhenAny(completion.Task, Task.Delay(effectiveTimeout, delayCts.Token)).ConfigureAwait(false);
            if (completed != completion.Task)
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException($"The protoo request '{method}' timed out after {effectiveTimeout.TotalSeconds:0.#} seconds.");
            }

            delayCts.Cancel();
            ProtooResponse response = await completion.Task.ConfigureAwait(false);
            KookDebugger.DebugAudio($"[Signaling] Received a response [Method] {method} [ID] {response.Id} [OK] {response.Ok} "
                + $"[Error Code] {response.ErrorCode} [Error Reason] {response.ErrorReason} [Data] {ProtooLogFormatter.RenderData(response.Data)}");
            if (!response.Ok)
            {
                throw new ProtooException(
                    $"The protoo request '{method}' failed: {response.ErrorCode} {response.ErrorReason}",
                    response.ErrorCode, response.ErrorReason);
            }
            return response.Data;
        }
        finally
        {
            _pending.TryRemove(id, out _);
        }
    }

    /// <summary>
    ///     发送请求并将响应的 <c>data</c> 反序列化为 <typeparamref name="T"/>。
    /// </summary>
    public async Task<T> RequestAsync<T>(string method, object? data = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        where T : class
    {
        JsonElement? result = await RequestAsync(method, data, timeout, cancellationToken).ConfigureAwait(false);
        if (result is null)
            throw new ProtooException($"The protoo request '{method}' returned no data.");
        T? value = result.Value.Deserialize<T>(ProtooJson.Options);
        return value ?? throw new ProtooException($"The protoo request '{method}' returned data that could not be parsed.");
    }

    public Task NotifyAsync(string method, object? data = null, CancellationToken cancellationToken = default)
    {
        Preconditions.NotNullOrWhitespace(method, nameof(method));
        KookDebugger.DebugAudio($"[Signaling] Sent a notification [Method] {method} [Data] {DescribeData(data)}");
        return _transport.SendTextAsync(ProtooMessageCodec.EncodeNotification(method, data), cancellationToken);
    }

    public Task CloseAsync(CancellationToken cancellationToken = default) => _transport.CloseAsync(cancellationToken);

    private static string DescribeData(object? data)
    {
        if (!KookDebugger.IsDebuggingAudio || data is null)
            return data is null ? "(none)" : string.Empty;
        JsonElement element = data is JsonElement json ? json : JsonSerializer.SerializeToElement(data, data.GetType(), ProtooJson.Options);
        return ProtooLogFormatter.RenderData(element);
    }

    private long RegisterPending(TaskCompletionSource<ProtooResponse> completion)
    {
        while (true)
        {
            long id;
            lock (_randomLock)
                id = _random.Next(1, 10_000_000);
            // protoo 的请求 ID 是随机数，理论上可能碰撞，这里避开仍在等待中的 ID
            if (_pending.TryAdd(id, completion))
                return id;
        }
    }

    // protoo 4.0.3：1500 × (15 + 0.1 × 待处理请求数) 毫秒，至少 22.5 秒
    private TimeSpan GetDefaultTimeout() => TimeSpan.FromMilliseconds(1500 * (15 + 0.1 * _pending.Count));

    private async Task OnTextReceivedAsync(string text)
    {
        ProtooMessage? message = ProtooMessageCodec.Decode(text);
        switch (message)
        {
            case ProtooResponse response:
                if (_pending.TryGetValue(response.Id, out TaskCompletionSource<ProtooResponse>? completion))
                    completion.TrySetResult(response);
                else
                    await LogDebugAsync($"Ignored a response for unknown request id {response.Id}.").ConfigureAwait(false);
                break;
            case ProtooRequest request:
                KookDebugger.DebugAudio($"[Signaling] Received a request [Method] {request.Method} [ID] {request.Id} "
                    + $"[Data] {ProtooLogFormatter.RenderData(request.Data)}");
                _ = Task.Run(() => HandleRequestAsync(request));
                break;
            case ProtooNotification notification:
                KookDebugger.DebugAudio($"[Signaling] Received a notification [Method] {notification.Method} "
                    + $"[Data] {ProtooLogFormatter.RenderData(notification.Data)}");
                await RaiseNotificationAsync(notification).ConfigureAwait(false);
                break;
            default:
                await LogDebugAsync("Ignored a frame that is not a protoo message.").ConfigureAwait(false);
                break;
        }
    }

    private async Task HandleRequestAsync(ProtooRequest request)
    {
        string reply;
        try
        {
            Func<ProtooRequest, Task<ProtooRequestResult>>? handler = RequestHandler;
            ProtooRequestResult result = handler is null
                ? ProtooRequestResult.Reject(404, "unknown method")
                : await handler(request).ConfigureAwait(false);
            reply = result.Accepted
                ? ProtooMessageCodec.EncodeResponse(request.Id, result.Data)
                : ProtooMessageCodec.EncodeErrorResponse(request.Id, result.ErrorCode ?? 500, result.ErrorReason ?? "rejected");
        }
        catch (Exception ex)
        {
            await LogWarningAsync($"Handling the protoo request '{request.Method}' failed.", ex).ConfigureAwait(false);
            reply = ProtooMessageCodec.EncodeErrorResponse(request.Id, 500, ex.Message);
        }

        try
        {
            await _transport.SendTextAsync(reply, CancellationToken.None).ConfigureAwait(false);
            KookDebugger.DebugAudio($"[Signaling] Replied to a request [Method] {request.Method} [ID] {request.Id}");
        }
        catch (Exception ex)
        {
            await LogWarningAsync($"Replying to the protoo request '{request.Method}' failed.", ex).ConfigureAwait(false);
        }
    }

    private async Task RaiseNotificationAsync(ProtooNotification notification)
    {
        Func<ProtooNotification, Task>? handler = NotificationReceived;
        if (handler is null)
            return;
        try
        {
            await handler(notification).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogWarningAsync($"Handling the protoo notification '{notification.Method}' failed.", ex).ConfigureAwait(false);
        }
    }

    private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_heartbeatInterval, cancellationToken).ConfigureAwait(false);
                await _transport.SendTextAsync(HeartbeatText, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常停止
        }
        catch (Exception ex)
        {
            // 发送失败通常意味着连接已断开，关闭事件会由传输层上报
            await LogDebugAsync("The heartbeat stopped.", ex).ConfigureAwait(false);
        }
    }

    private async Task OnTransportClosedAsync(ProtooCloseInfo info)
    {
        if (Interlocked.Exchange(ref _closed, 1) == 1)
            return;

        _heartbeatCts?.Cancel();
        ProtooException closedException = new(
            $"The protoo connection was closed (code: {info.Code?.ToString() ?? "none"}, reason: {info.Reason ?? "none"}).",
            info.Code, info.Reason, info.Error);
        foreach (KeyValuePair<long, TaskCompletionSource<ProtooResponse>> pair in _pending)
            pair.Value.TrySetException(closedException);

        Func<ProtooCloseInfo, Task>? handler = Closed;
        if (handler is null)
            return;
        try
        {
            await handler(info).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogWarningAsync("Handling the protoo close event failed.", ex).ConfigureAwait(false);
        }
    }

    private Task LogDebugAsync(string message, Exception? exception = null) =>
        _logger?.DebugAsync(message, exception) ?? Task.CompletedTask;

    private Task LogWarningAsync(string message, Exception? exception = null) =>
        _logger?.WarningAsync(message, exception) ?? Task.CompletedTask;

    public void Dispose()
    {
        _transport.TextReceived -= OnTextReceivedAsync;
        _transport.Closed -= OnTransportClosedAsync;
        _heartbeatCts?.Cancel();
        _heartbeatCts?.Dispose();
        _transport.Dispose();
    }
}
