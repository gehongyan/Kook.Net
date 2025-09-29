#if NET462
using System.Net.Http;
#endif

using System.Collections.Immutable;
using Kook.Logging;

namespace Kook.Rest;

/// <summary>
///     表示一个可以连接到 KOOK API 的通用的 KOOK 客户端。
/// </summary>
public abstract class BaseKookClient : IKookClient
{
    #region BaseKookClient

    /// <summary>
    ///     当生成一条日志消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="Kook.LogMessage"/> 参数是描述日志消息的结构。 </item>
    ///     </list>
    /// </remarks>
    public event Func<LogMessage, Task> Log
    {
        add => _logEvent.Add(value);
        remove => _logEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new();

    /// <summary>
    ///     当客户端登录成功时引发。
    /// </summary>
    public event Func<Task> LoggedIn
    {
        add => _loggedInEvent.Add(value);
        remove => _loggedInEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Task>> _loggedInEvent = new();

    /// <summary>
    ///     当客户端退出登录时引发。
    /// </summary>
    public event Func<Task> LoggedOut
    {
        add => _loggedOutEvent.Add(value);
        remove => _loggedOutEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<Task>> _loggedOutEvent = new();

    /// <summary>
    ///     当向 API 发送 REST 请求时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.Net.Http.HttpMethod"/> 参数是 HTTP 方法。 </item>
    ///     <item> <see cref="System.String"/> 参数是终结点。 </item>
    ///     <item> <see cref="System.Double"/> 参数是完成请求所花费的时间，以毫秒为单位。 </item>
    ///     </list>
    /// </remarks>
    public event Func<HttpMethod, string, double, Task> SentRequest
    {
        add => _sentRequest.Add(value);
        remove => _sentRequest.Remove(value);
    }

    internal readonly AsyncEvent<Func<HttpMethod, string, double, Task>> _sentRequest = new();

    internal readonly Logger _restLogger;
    private readonly SemaphoreSlim _stateLock;
    private bool _isFirstLogin, _isDisposed;

    internal API.KookRestApiClient ApiClient { get; }

    internal LogManager LogManager { get; }

    /// <summary>
    ///     获取此客户端的登录状态。
    /// </summary>
    public LoginState LoginState { get; protected set; }

    /// <summary>
    ///     获取登录到此客户端的当前用户；如果未登录，则为 <c>null</c>。
    /// </summary>
    public ISelfUser? CurrentUser { get; protected set; }

    /// <inheritdoc />
    public TokenType TokenType => ApiClient.AuthTokenType;

    internal bool FormatUsersInBidirectionalUnicode { get; private set; }

    internal BaseKookClient(KookRestConfig config, API.KookRestApiClient client)
    {
        ApiClient = client;
        LogManager = new LogManager(config.LogLevel);
        LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

        _stateLock = new SemaphoreSlim(1, 1);
        _restLogger = LogManager.CreateLogger("Rest");
        _isFirstLogin = config.DisplayInitialLog;

        FormatUsersInBidirectionalUnicode = config.FormatUsersInBidirectionalUnicode;

        ApiClient.RequestQueue.RateLimitTriggered += async (id, info, endpoint) =>
        {
            if (info == null)
                await _restLogger.VerboseAsync($"Preemptive Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
            else
                await _restLogger.WarningAsync($"Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
        };
        ApiClient.SentRequest += async (method, endpoint, millis) =>
            await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        ApiClient.SentRequest += (method, endpoint, millis) =>
            _sentRequest.InvokeAsync(method, endpoint, millis);
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
    public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await LoginInternalAsync(tokenType, token, validateToken).ConfigureAwait(false);
        }
        finally
        {
            _stateLock.Release();
        }
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

    internal virtual Task OnLoginAsync(TokenType tokenType, string token) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task LogoutAsync()
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await LogoutInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _stateLock.Release();
        }
    }

    internal virtual async Task LogoutInternalAsync()
    {
        if (LoginState == LoginState.LoggedOut)
            return;
        LoginState = LoginState.LoggingOut;
        await ApiClient.LogoutAsync().ConfigureAwait(false);
        await OnLogoutAsync().ConfigureAwait(false);
        CurrentUser = null;
        LoginState = LoginState.LoggedOut;
        await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
    }

    internal virtual Task OnLogoutAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public virtual ConnectionState ConnectionState => ConnectionState.Disconnected;

    #endregion

    #region IKookClient

    /// <inheritdoc />
    ISelfUser? IKookClient.CurrentUser => CurrentUser;

    /// <inheritdoc />
    Task<IGuild?> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuild?>(null);

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IGuild>>(ImmutableArray.Create<IGuild>());

    /// <inheritdoc />
    Task<IChannel?> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IChannel?>(null);

    /// <inheritdoc />
    Task<IDMChannel?> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IDMChannel?>(null);

    /// <inheritdoc />
    Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IDMChannel>>(ImmutableArray.Create<IDMChannel>());

    /// <inheritdoc />
    Task<IUser?> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(null);

    /// <inheritdoc />
    Task<IUser?> IKookClient.GetUserAsync(string username, string identifyNumber, RequestOptions? options) =>
        Task.FromResult<IUser?>(null);

    /// <inheritdoc />
    Task<IReadOnlyCollection<IUser>> IKookClient.GetFriendsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IUser>>(ImmutableArray.Create<IUser>());

    /// <inheritdoc />
    Task<IReadOnlyCollection<IFriendRequest>> IKookClient.GetFriendRequestsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IFriendRequest>>(ImmutableArray.Create<IFriendRequest>());

    /// <inheritdoc />
    Task<IReadOnlyCollection<IUser>> IKookClient.GetBlockedUsersAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IUser>>(ImmutableArray.Create<IUser>());

    /// <inheritdoc />
    Task<IReadOnlyCollection<IIntimacyRelation>> IKookClient.GetIntimacyRelationsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IIntimacyRelation>>(ImmutableArray.Create<IIntimacyRelation>());

    /// <inheritdoc />
    Task<IReadOnlyCollection<IFriendRequest>> IKookClient.GetIntimacyRelationRequestsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IFriendRequest>>(ImmutableArray.Create<IFriendRequest>());

    /// <inheritdoc />
    Task IKookClient.StartAsync() =>
        Task.CompletedTask;

    /// <inheritdoc />
    Task IKookClient.StopAsync() =>
        Task.CompletedTask;

    #endregion
}
