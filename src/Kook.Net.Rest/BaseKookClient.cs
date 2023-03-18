using Kook.Logging;
using System.Collections.Immutable;

namespace Kook.Rest;

public abstract class BaseKookClient : IKookClient
{
    #region BaseKookClient
    public event Func<LogMessage, Task> Log { add => _logEvent.Add(value); remove => _logEvent.Remove(value); }
    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();
    public event Func<Task> LoggedIn { add => _loggedInEvent.Add(value); remove => _loggedInEvent.Remove(value); }
    private readonly AsyncEvent<Func<Task>> _loggedInEvent = new AsyncEvent<Func<Task>>();
    public event Func<Task> LoggedOut { add => _loggedOutEvent.Add(value); remove => _loggedOutEvent.Remove(value); }
    private readonly AsyncEvent<Func<Task>> _loggedOutEvent = new AsyncEvent<Func<Task>>();

    internal readonly Logger _restLogger;
    private readonly SemaphoreSlim _stateLock;
    private bool _isFirstLogin, _isDisposed;

    internal API.KookRestApiClient ApiClient { get; }

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

    internal BaseKookClient(KookRestConfig config, API.KookRestApiClient client)
    {
        ApiClient = client;
        LogManager = new LogManager(config.LogLevel);
        LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

        _stateLock = new SemaphoreSlim(1, 1);
        _restLogger = LogManager.CreateLogger("Rest");
        _isFirstLogin = config.DisplayInitialLog;

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
                    await LogManager.WarningAsync("Kook", "A supplied token was invalid.", ex).ConfigureAwait(false);
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
            await LogoutInternalAsync().ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }
    internal virtual async Task LogoutInternalAsync()
    {
        await ApiClient.GoOfflineAsync();

        if (LoginState == LoginState.LoggedOut)
            return;
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

    #region IKookClient

    /// <inheritdoc />
    ConnectionState IKookClient.ConnectionState => ConnectionState.Disconnected;

    /// <inheritdoc />
    ISelfUser IKookClient.CurrentUser => CurrentUser;

    /// <inheritdoc />
    Task<IGuild> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuild>(null);
    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IGuild>>(ImmutableArray.Create<IGuild>());
    /// <inheritdoc />
    Task<IChannel> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IChannel>(null);
    /// <inheritdoc />
    Task<IDMChannel> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions options)
        => Task.FromResult<IDMChannel>(null);
    /// <inheritdoc />
    Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IDMChannel>>(ImmutableArray.Create<IDMChannel>());
    /// <inheritdoc />
    Task<IUser> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null);
    /// <inheritdoc />
    Task<IUser> IKookClient.GetUserAsync(string username, string identifyNumber, RequestOptions options)
        => Task.FromResult<IUser>(null);

    /// <inheritdoc />
    Task IKookClient.StartAsync()
        => Task.Delay(0);
    /// <inheritdoc />
    Task IKookClient.StopAsync()
        => Task.Delay(0);

    #endregion
}
