using Kook.Logging;
using Kook.Net;

namespace Kook;

internal class ConnectionManager : IDisposable
{
    public event Func<Task> Connected
    {
        add => _connectedEvent.Add(value);
        remove => _connectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Task>> _connectedEvent = new();

    public event Func<Exception, bool, Task> Disconnected
    {
        add => _disconnectedEvent.Add(value);
        remove => _disconnectedEvent.Remove(value);
    }

    private readonly AsyncEvent<Func<Exception, bool, Task>> _disconnectedEvent = new();

    private readonly SemaphoreSlim _stateLock;
    private readonly Logger _logger;
    private readonly int _connectionTimeout;
    private readonly Func<Task> _onConnecting;
    private readonly Func<Exception, Task> _onDisconnecting;

    private TaskCompletionSource<bool>? _connectionPromise;
    private TaskCompletionSource<bool>? _readyPromise;
    private CancellationTokenSource? _combinedCancellationToken;
    private CancellationTokenSource? _reconnectCancellationToken;
    private CancellationTokenSource? _connectionCancellationToken;

    private bool _isDisposed;

    public ConnectionState State { get; private set; }
    public CancellationToken CancellationToken { get; private set; }

    internal ConnectionManager(SemaphoreSlim stateLock, Logger logger,
        int connectionTimeout, Func<Task> onConnecting, Func<Exception, Task> onDisconnecting,
        Action<Func<Exception, Task>> clientDisconnectHandler)
    {
        _stateLock = stateLock;
        _logger = logger;
        _connectionTimeout = connectionTimeout;
        _onConnecting = onConnecting;
        _onDisconnecting = onDisconnecting;

        clientDisconnectHandler(ex =>
        {
            if (ex != null)
            {
                WebSocketClosedException? ex2 = ex as WebSocketClosedException;
                if (ex2?.CloseCode == 4006)
                    CriticalError(new Exception("WebSocket session expired", ex));
                else if (ex2?.CloseCode == 4014)
                    CriticalError(new Exception("WebSocket connection was closed", ex));
                else
                    Error(new Exception("WebSocket connection was closed", ex));
            }
            else
                Error(new Exception("WebSocket connection was closed"));

            return Task.CompletedTask;
        });
    }

    public virtual async Task StartAsync()
    {
        if (State != ConnectionState.Disconnected)
            throw new InvalidOperationException("Cannot start an already running client.");

        await AcquireConnectionLock().ConfigureAwait(false);
        CancellationTokenSource reconnectCancellationToken = new();
        _reconnectCancellationToken?.Dispose();
        _reconnectCancellationToken = reconnectCancellationToken;
        _ = Task.Run(async () =>
        {
            try
            {
                Random jitter = new();
                int nextReconnectDelay = 1000;
                while (!reconnectCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await ConnectAsync(reconnectCancellationToken).ConfigureAwait(false);
                        nextReconnectDelay = 1000; //Reset delay
                        if (_connectionPromise is null)
                            await _logger.ErrorAsync("The connection promise was null after connecting").ConfigureAwait(false);
                        else
                            await _connectionPromise.Task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Error(ex); //In case this exception didn't come from another Error call
                        if (!reconnectCancellationToken.IsCancellationRequested)
                        {
                            await _logger.WarningAsync(ex).ConfigureAwait(false);
                            await DisconnectAsync(ex, true).ConfigureAwait(false);
                        }
                        else
                        {
                            await _logger.ErrorAsync(ex).ConfigureAwait(false);
                            await DisconnectAsync(ex, false).ConfigureAwait(false);
                        }
                    }

                    if (!reconnectCancellationToken.IsCancellationRequested)
                    {
                        //Wait before reconnecting
                        await Task.Delay(nextReconnectDelay, reconnectCancellationToken.Token).ConfigureAwait(false);
                        nextReconnectDelay = nextReconnectDelay * 2 + jitter.Next(-250, 250);
                        if (nextReconnectDelay > 60000)
                            nextReconnectDelay = 60000;
                    }
                }
            }
            finally
            {
                _stateLock.Release();
            }
        }, CancellationToken.None);
    }

    public virtual Task StopAsync()
    {
        Cancel();
        return Task.CompletedTask;
    }

    private async Task ConnectAsync(CancellationTokenSource reconnectCancellationToken)
    {
        _connectionCancellationToken?.Dispose();
        _combinedCancellationToken?.Dispose();
        _connectionCancellationToken = new CancellationTokenSource();
        _combinedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(_connectionCancellationToken.Token, reconnectCancellationToken.Token);
        CancellationToken = _combinedCancellationToken.Token;

        _connectionPromise = new TaskCompletionSource<bool>();
        State = ConnectionState.Connecting;
        await _logger.InfoAsync("Connecting").ConfigureAwait(false);

        try
        {
            TaskCompletionSource<bool> readyPromise = new();
            _readyPromise = readyPromise;

            //Abort connection on timeout
            CancellationToken cancellationToken = CancellationToken;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_connectionTimeout, cancellationToken).ConfigureAwait(false);
                    readyPromise.TrySetException(new TimeoutException());
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
            }, CancellationToken.None);

            await _onConnecting().ConfigureAwait(false);

            await _logger.InfoAsync("Connected").ConfigureAwait(false);
            State = ConnectionState.Connected;
            await _logger.DebugAsync("Raising Event").ConfigureAwait(false);
            await _connectedEvent.InvokeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }
    }

    private async Task DisconnectAsync(Exception ex, bool isReconnecting)
    {
        if (State == ConnectionState.Disconnected) return;

        State = ConnectionState.Disconnecting;
        await _logger.InfoAsync("Disconnecting").ConfigureAwait(false);

        await _onDisconnecting(ex).ConfigureAwait(false);

        State = ConnectionState.Disconnected;
        await _disconnectedEvent.InvokeAsync(ex, isReconnecting).ConfigureAwait(false);
        await _logger.InfoAsync("Disconnected").ConfigureAwait(false);
    }

    public Task CompleteAsync()
    {
        _readyPromise?.TrySetResult(true);
        return Task.CompletedTask;
    }

    public async Task WaitAsync()
    {
        if (_readyPromise is not null)
            await _readyPromise.Task.ConfigureAwait(false);
    }

    public void Cancel()
    {
        _readyPromise?.TrySetCanceled();
        _connectionPromise?.TrySetCanceled();
        _reconnectCancellationToken?.Cancel();
        _connectionCancellationToken?.Cancel();
    }

    public void Error(Exception ex)
    {
        _readyPromise?.TrySetException(ex);
        _connectionPromise?.TrySetException(ex);
        _connectionCancellationToken?.Cancel();
    }

    public void CriticalError(Exception ex)
    {
        _reconnectCancellationToken?.Cancel();
        Error(ex);
    }

    public void Reconnect()
    {
        _readyPromise?.TrySetCanceled();
        _connectionPromise?.TrySetCanceled();
        _connectionCancellationToken?.Cancel();
    }

    private async Task AcquireConnectionLock()
    {
        while (true)
        {
            await StopAsync().ConfigureAwait(false);
            if (await _stateLock.WaitAsync(0, CancellationToken.None).ConfigureAwait(false))
                break;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _combinedCancellationToken?.Dispose();
            _reconnectCancellationToken?.Dispose();
            _connectionCancellationToken?.Dispose();
        }

        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);
}
