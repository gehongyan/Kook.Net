using System.Collections.Immutable;
using KaiHeiLa.Logging;

namespace KaiHeiLa.Rest;

public abstract class BaseKaiHeiLaClient : IKaiHeiLaClient
{
    #region BaseKaiHeiLaClient
    public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();
    public event Func<Task> LoggedIn { add { _loggedInEvent.Add(value); } remove { _loggedInEvent.Remove(value); } }
    private readonly AsyncEvent<Func<Task>> _loggedInEvent = new AsyncEvent<Func<Task>>();
    public event Func<Task> LoggedOut { add { _loggedOutEvent.Add(value); } remove { _loggedOutEvent.Remove(value); } }
    private readonly AsyncEvent<Func<Task>> _loggedOutEvent = new AsyncEvent<Func<Task>>();
    
    internal readonly Logger _restLogger;
    private readonly SemaphoreSlim _stateLock;
    private bool _isFirstLogin, _isDisposed;
    
    internal API.KaiHeiLaRestApiClient ApiClient { get; }
    
    internal LogManager LogManager { get; }
    /// <summary>
    ///     Gets the login state of the client.
    /// </summary>
    public LoginState LoginState { get; private set; }
    /// <summary>
    ///     Gets the logged-in user.
    /// </summary>
    public ISelfUser CurrentUser { get; protected set; }
    /// <inheritdoc />
    public TokenType TokenType => ApiClient.AuthTokenType;
    
    internal BaseKaiHeiLaClient(KaiHeiLaRestConfig config, API.KaiHeiLaRestApiClient client)
    {
        ApiClient = client;
        LogManager = new LogManager(config.LogLevel);
        LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
        
        _stateLock = new SemaphoreSlim(1, 1);
        _restLogger = LogManager.CreateLogger("Rest");
        
        ApiClient.RequestQueue.RateLimitTriggered += async (id, info, endpoint) =>
        {
            if (info == null)
                await _restLogger.VerboseAsync($"Preemptive Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
            else
                await _restLogger.WarningAsync($"Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
        };
        ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
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

    public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await LoginInternalAsync(tokenType, token, validateToken).ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }
    
    internal virtual async Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken)
    {
        if (_isFirstLogin)
        {
            _isFirstLogin = false;
            await LogManager.WriteInitialLog().ConfigureAwait(false);
        }

        if (LoginState != LoginState.LoggedOut)
            await LogoutInternalAsync().ConfigureAwait(false);
        LoginState = LoginState.LoggingIn;

        try
        {
            // If token validation is enabled, validate the token and let it throw any ArgumentExceptions
            // that result from invalid parameters
            if (validateToken)
            {
                try
                {
                    TokenUtils.ValidateToken(tokenType, token);
                }
                catch (ArgumentException ex)
                {
                    // log these ArgumentExceptions and allow for the client to attempt to log in anyways
                    await LogManager.WarningAsync("KaiHeiLa", "A supplied token was invalid.", ex).ConfigureAwait(false);
                }
            }

            await ApiClient.LoginAsync(tokenType, token).ConfigureAwait(false);
            await OnLoginAsync(tokenType, token).ConfigureAwait(false);
            LoginState = LoginState.LoggedIn;
        }
        catch
        {
            await LogoutInternalAsync().ConfigureAwait(false);
            throw;
        }

        await _loggedInEvent.InvokeAsync().ConfigureAwait(false);
    }
    internal virtual Task OnLoginAsync(TokenType tokenType, string token)
        => Task.Delay(0);
    
    public async Task LogoutAsync()
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await ApiClient.GoOfflineAsync();
            await LogoutInternalAsync().ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }
    internal virtual async Task LogoutInternalAsync()
    {
        if (LoginState == LoginState.LoggedOut) return;
        LoginState = LoginState.LoggingOut;

        await ApiClient.LogoutAsync().ConfigureAwait(false);

        await OnLogoutAsync().ConfigureAwait(false);
        CurrentUser = null;
        LoginState = LoginState.LoggedOut;

        await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
    }
    internal virtual Task OnLogoutAsync()
        => Task.Delay(0);
    
    #endregion

    #region IKaiHeiLaClient
    
    /// <inheritdoc />
    ConnectionState IKaiHeiLaClient.ConnectionState => ConnectionState.Disconnected;
    
    /// <inheritdoc />
    ISelfUser IKaiHeiLaClient.CurrentUser => CurrentUser;

    /// <inheritdoc />
    Task<IGuild> IKaiHeiLaClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuild>(null);
    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKaiHeiLaClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IGuild>>(ImmutableArray.Create<IGuild>());
    /// <inheritdoc />
    public Task<IChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        => Task.FromResult<IChannel>(null);
    /// <inheritdoc />
    public Task<IDMChannel> GetDMChannelAsync(Guid chatCode, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        => Task.FromResult<IDMChannel>(null);
    /// <inheritdoc />
    public Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        => Task.FromResult<IReadOnlyCollection<IDMChannel>>(ImmutableArray.Create<IDMChannel>());
    /// <inheritdoc />
    Task<IUser> IKaiHeiLaClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null);
    
    /// <inheritdoc />
    Task IKaiHeiLaClient.StartAsync()
        => Task.Delay(0);
    /// <inheritdoc />
    Task IKaiHeiLaClient.StopAsync()
        => Task.Delay(0);
    
    #endregion
}