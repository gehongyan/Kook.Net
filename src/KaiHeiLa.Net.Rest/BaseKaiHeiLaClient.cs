using KaiHeiLa.Logging;

namespace KaiHeiLa.Rest;

public abstract class BaseKaiHeiLaClient : IKaiHeiLaClient
{
    #region BaseKaiHeiLaClient
    public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();
    
    private readonly SemaphoreSlim _stateLock;
    private bool _isDisposed;
    
    internal API.KaiHeiLaRestApiClient ApiClient { get; }
    
    internal LogManager LogManager { get; }
    
    internal BaseKaiHeiLaClient(KaiHeiLaRestConfig config, API.KaiHeiLaRestApiClient client)
    {
        ApiClient = client;
        LogManager = new LogManager(config.LogLevel);
        LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
        
        _stateLock = new SemaphoreSlim(1, 1);
    }
    
    internal virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            ApiClient.Dispose();
            _stateLock?.Dispose();
            _isDisposed = true;
        }
    }
    /// <inheritdoc />
    public void Dispose() => Dispose(true);
    
    /// <inheritdoc />
    Task IKaiHeiLaClient.StartAsync()
        => Task.Delay(0);
    /// <inheritdoc />
    Task IKaiHeiLaClient.StopAsync()
        => Task.Delay(0);
    
    #endregion
}