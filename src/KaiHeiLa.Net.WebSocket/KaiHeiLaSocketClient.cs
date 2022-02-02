using System.Collections.Concurrent;
using System.Text.Json;
using KaiHeiLa.API;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.Logging;

namespace KaiHeiLa.WebSocket;

public partial class KaiHeiLaSocketClient : BaseSocketClient
{
    #region KaiHeiLaSocketClient

    private readonly JsonSerializerOptions _serializerOptions;

    private readonly ConcurrentQueue<long> _heartbeatTimes;
    private readonly ConnectionManager _connection;
    private readonly Logger _gatewayLogger;
    private readonly SemaphoreSlim _stateLock;

    private Guid _sessionId;
    private int _lastSeq = 0;
    private Task _heartbeatTask;
    private long _lastMessageTime;

    private bool _isDisposed;

    public ConnectionState ConnectionState => _connection.State;
    public override int Latency { get; protected set; }

    #endregion

    // From KaiHeiLaSocketConfig
    internal ClientState State { get; private set; }
    internal int? HandlerTimeout { get; private set; }
    internal new KaiHeiLaSocketApiClient ApiClient => base.ApiClient;

    public KaiHeiLaSocketClient(KaiHeiLaSocketConfig config) : this(config, CreateApiClient(config))
    {
    }

    private KaiHeiLaSocketClient(KaiHeiLaSocketConfig config, KaiHeiLaSocketApiClient client)
        : base(config, client)
    {
        HandlerTimeout = config.HandlerTimeout;
        State = new ClientState(0, 0);
        _heartbeatTimes = new ConcurrentQueue<long>();

        _stateLock = new SemaphoreSlim(1, 1);
        _gatewayLogger = LogManager.CreateLogger("Gateway");
        _connection = new ConnectionManager(_stateLock, _gatewayLogger, config.ConnectionTimeout,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        _connection.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
        _connection.Disconnected += (ex, recon) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);

        _serializerOptions = client.SerializerOptions;

        ApiClient.SentGatewayMessage += async socketFrameType =>
            await _gatewayLogger.DebugAsync($"Sent {socketFrameType}").ConfigureAwait(false);
        ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;
    }

    private static KaiHeiLaSocketApiClient CreateApiClient(KaiHeiLaSocketConfig config)
        => new KaiHeiLaSocketApiClient();

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                StopAsync().GetAwaiter().GetResult();
                ApiClient?.Dispose();
                _stateLock?.Dispose();
            }

            _isDisposed = true;
        }

        base.Dispose(disposing);
    }

    private async Task OnConnectingAsync()
    {
        try
        {
            await _gatewayLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
            await ApiClient.ConnectAsync().ConfigureAwait(false);
        }
        catch
        {
        }

        await _connection.WaitAsync().ConfigureAwait(false);
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _gatewayLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
        await ApiClient.DisconnectAsync(ex).ConfigureAwait(false);

        //Wait for tasks to complete
        await _gatewayLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
        var heartbeatTask = _heartbeatTask;
        if (heartbeatTask != null)
            await heartbeatTask.ConfigureAwait(false);
        _heartbeatTask = null;

        while (_heartbeatTimes.TryDequeue(out _))
        {
        }

        _lastMessageTime = 0;
    }


    private async Task ProcessMessageAsync(SocketFrameType socketFrameType, int? sequence, object payload)
    {
        if (sequence != null)
            _lastSeq = sequence.Value;
        _lastMessageTime = Environment.TickCount;

        try
        {
            switch (socketFrameType)
            {
                case SocketFrameType.Event:
                    GatewayEvent gatewayEvent =
                        ((JsonElement) payload).Deserialize<GatewayEvent>(_serializerOptions);

                    dynamic eventExtraData = gatewayEvent!.Type switch
                    {
                        MessageType.System => ((JsonElement) gatewayEvent.ExtraData)
                            .Deserialize<GatewaySystemEventExtraData>(_serializerOptions),
                        _ => ((JsonElement) gatewayEvent.ExtraData).Deserialize<GatewayMessageExtraData>(
                            _serializerOptions)
                    };

                    switch (gatewayEvent.Type)
                    {
                        case MessageType.Text:
                        {
                            await _gatewayLogger.DebugAsync("Received Message (Text)").ConfigureAwait(false);
                            GatewayMessageExtraData extraData = eventExtraData as GatewayMessageExtraData;
                        }

                            break;
                        // case MessageType.Image:
                        //     break;
                        // case MessageType.Video:
                        //     break;
                        // case MessageType.File:
                        //     break;
                        // case MessageType.Audio:
                        //     break;
                        // case MessageType.KMarkdown:
                        //     break;
                        // case MessageType.Card:
                        //     break;
                        case MessageType.System:
                        {
                            GatewaySystemEventExtraData extraData = eventExtraData as GatewaySystemEventExtraData;
                            switch (gatewayEvent.ChannelType, extraData!.Type)
                            {
                                #region Channels

                                // 频道内用户添加 reaction
                                case ("GROUP", "added_reaction"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (added_reaction)")
                                        .ConfigureAwait(false);
                                    ReactionAddedInChannel body =
                                        ((JsonElement) extraData.Body).Deserialize<ReactionAddedInChannel>(
                                            _serializerOptions);
                                    // TODO: 频道内用户添加 reaction
                                }
                                    break;

                                #endregion

                                default:
                                    await _gatewayLogger.WarningAsync($"Unknown System Event Type ({extraData.Type})")
                                        .ConfigureAwait(false);
                                    break;
                            }
                        }
                            break;
                        default:
                            await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type})")
                                .ConfigureAwait(false);
                            break;
                    }

                    break;

                case SocketFrameType.Hello:
                    await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                    try
                    {
                        GatewayHelloPayload gatewayHelloPayload =
                            ((JsonElement) payload).Deserialize<GatewayHelloPayload>(_serializerOptions);
                        _sessionId = gatewayHelloPayload?.SessionId ?? Guid.Empty;
                        _heartbeatTask = RunHeartbeatAsync(_connection.CancelToken);
                    }
                    catch (Exception ex)
                    {
                        _connection.CriticalError(new Exception("Processing Hello failed", ex));
                        return;
                    }

                    _ = _connection.CompleteAsync();
                    break;

                case SocketFrameType.Pong:
                    await _gatewayLogger.DebugAsync("Received Pong").ConfigureAwait(false);
                    if (_heartbeatTimes.TryDequeue(out long time))
                    {
                        int latency = (int) (Environment.TickCount - time);
                        int before = Latency;
                        Latency = latency;

                        await TimedInvokeAsync(_latencyUpdatedEvent, nameof(LatencyUpdated), before, latency)
                            .ConfigureAwait(false);
                    }

                    break;

                case SocketFrameType.Reconnect:
                    await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                    _connection.Error(new GatewayReconnectException("Server requested a reconnect"));
                    break;

                case SocketFrameType.ResumeAck:
                    await _gatewayLogger.DebugAsync("Received ResumeAck").ConfigureAwait(false);
                    _ = _connection.CompleteAsync();
                    await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                    break;

                default:
                    await _gatewayLogger.WarningAsync($"Unknown Socket Frame Type ({socketFrameType})")
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync($"Error handling {socketFrameType}", ex).ConfigureAwait(false);
        }
    }

    public Task LoginAsync(TokenType tokenType, string token)
    {
        ApiClient.LoginAsync(tokenType, token);
        ApiClient.LoginState = LoginState.LoggedIn;
        return Task.CompletedTask;
    }

    public override async Task StartAsync() => await _connection.StartAsync().ConfigureAwait(false);
    public override async Task StopAsync() => await _connection.StopAsync().ConfigureAwait(false);

    private async Task RunHeartbeatAsync(CancellationToken cancelToken)
    {
        int intervalMillis = KaiHeiLaSocketConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancelToken.IsCancellationRequested)
            {
                int now = Environment.TickCount;

                //Did server respond to our last heartbeat, or are we still receiving messages (long load?)
                if (_heartbeatTimes.IsEmpty && (now - _lastMessageTime) > intervalMillis)
                {
                    if (ConnectionState == ConnectionState.Connected)
                    {
                        _connection.Error(new GatewayReconnectException("Server missed last heartbeat"));
                        return;
                    }
                }

                _heartbeatTimes.Enqueue(now);
                try
                {
                    await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _gatewayLogger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                }

                await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
            }

            await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync(AsyncEvent<Func<Task>> eventHandler, string name)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, eventHandler.InvokeAsync).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync().ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T>(AsyncEvent<Func<T, Task>> eventHandler, string name, T arg)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2>(AsyncEvent<Func<T1, T2, Task>> eventHandler, string name, T1 arg1,
        T2 arg2)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3>(AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, string name,
        T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3, T4>(AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3, T4, T5>(AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5))
                    .ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }
    }

    private async Task TimeoutWrap(string name, Func<Task> action)
    {
        try
        {
            var timeoutTask = Task.Delay(HandlerTimeout.Value);
            var handlersTask = action();
            if (await Task.WhenAny(timeoutTask, handlersTask).ConfigureAwait(false) == timeoutTask)
            {
                await _gatewayLogger.WarningAsync($"A {name} handler is blocking the gateway task.")
                    .ConfigureAwait(false);
            }

            await handlersTask.ConfigureAwait(false); //Ensure the handler completes
        }
        catch (Exception ex)
        {
            await _gatewayLogger.WarningAsync($"A {name} handler has thrown an unhandled exception.", ex)
                .ConfigureAwait(false);
        }
    }
}