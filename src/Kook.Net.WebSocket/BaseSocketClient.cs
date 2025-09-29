using Kook.API;
using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的客户端的抽象基类。
/// </summary>
public abstract partial class BaseSocketClient : BaseKookClient, IKookClient
{
    /// <summary>
    ///     获取此客户端使用的配置。
    /// </summary>
    protected readonly KookSocketConfig BaseConfig;

    /// <summary>
    ///     获取到网关服务器的往返延迟估计值（以毫秒为单位）。
    /// </summary>
    /// <remarks>
    ///     此往返估计值源于心跳包的延迟，不代表诸如发送消息等操作的真实延迟。
    /// </remarks>
    public abstract int Latency { get; protected set; }

    /// <summary>
    ///     获取一个与此客户端共享状态的仅限于 REST 的客户端。
    /// </summary>
    public abstract KookSocketRestClient Rest { get; }

    internal new KookSocketApiClient ApiClient => base.ApiClient as KookSocketApiClient
        ?? throw new InvalidOperationException("The API client is not a WebSocket-based client.");

    /// <inheritdoc cref="Kook.Rest.BaseKookClient.CurrentUser" />
    public new virtual SocketSelfUser? CurrentUser
    {
        get => base.CurrentUser as SocketSelfUser;
        protected set => base.CurrentUser = value;
    }

    /// <summary>
    ///     获取当前用户所在的所有服务器。
    /// </summary>
    public abstract IReadOnlyCollection<SocketGuild> Guilds { get; }

    internal BaseSocketClient(KookSocketConfig config, KookRestApiClient client)
        : base(config, client)
    {
        BaseConfig = config;
    }

    /// <summary>
    ///     获取用户。
    /// </summary>
    /// <remarks>
    ///     此方法可能返回 <c>null</c>，因为此方法仅会返回网关缓存中存在的用户，如果在当前 Bot
    ///     登录会话中，要获取的用户未引发过任何事件，那么该用户实体则不会存在于缓存中。
    /// </remarks>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的用户；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketUser? GetUser(ulong id);

    /// <summary>
    ///     获取用户。
    /// </summary>
    /// <remarks>
    ///     此方法可能返回 <c>null</c>，因为此方法仅会返回网关缓存中存在的用户，如果在当前 Bot
    ///     登录会话中，要获取的用户未引发过任何事件，那么该用户实体则不会存在于缓存中。
    /// </remarks>
    /// <param name="username"> 用户的名称。 </param>
    /// <param name="identifyNumber"> 用户的识别号。 </param>
    /// <returns> 与指定的名称和识别号关联的用户；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketUser? GetUser(string username, string identifyNumber);

    /// <summary>
    ///     获取一个服务器频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketChannel? GetChannel(ulong id);

    /// <summary>
    ///     获取一个私聊频道。
    /// </summary>
    /// <param name="chatCode"> 私聊频道的聊天代码。 </param>
    /// <returns> 具有指定聊天代码的私聊频道；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketDMChannel? GetDMChannel(Guid chatCode);

    /// <summary>
    ///     获取一个私聊频道。
    /// </summary>
    /// <param name="userId"> 私聊频道中另一位用户的 ID。 </param>
    /// <returns> 另一位用户具有指定用户 ID 的私聊频道；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketDMChannel? GetDMChannel(ulong userId);

    /// <summary>
    ///     获取一个服务器。
    /// </summary>
    /// <param name="id"> 要获取的服务器的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的服务器；如果未找到，则返回 <c>null</c>。 </returns>
    public abstract SocketGuild? GetGuild(ulong id);

    /// <inheritdoc />
    internal override async Task OnLoginAsync(TokenType tokenType, string token)
    {
        if (BaseConfig.AutoLogoutBeforeLogin)
            await LogoutTokenAsync();
        await base.OnLoginAsync(tokenType, token);
    }

    /// <inheritdoc />
    internal override async Task OnLogoutAsync()
    {
        await LogoutTokenAsync();
        await base.OnLogoutAsync();
    }

    private async Task LogoutTokenAsync()
    {
        await ApiClient
            .GoOfflineAsync(new RequestOptions { IgnoreState = true})
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public abstract Task StartAsync();

    /// <inheritdoc />
    public abstract Task StopAsync();

    /// <summary>
    ///     下载全部或指定服务器的用户到缓存中。
    /// </summary>
    /// <param name="guilds"> 要下载用户的服务器。如果为 <c>null</c>，则下载所有可用的服务器。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步下载操作的任务。 </returns>
    public abstract Task DownloadUsersAsync(IEnumerable<IGuild>? guilds = null, RequestOptions? options = null);

    /// <summary>
    ///     下载全部或指定服务器的语音状态到缓存中。
    /// </summary>
    /// <param name="guilds"> 要下载语音状态的服务器。如果为 <c>null</c>，则下载所有可用的服务器。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步下载操作的任务。 </returns>
    public abstract Task DownloadVoiceStatesAsync(IEnumerable<IGuild>? guilds = null, RequestOptions? options = null);

    /// <summary>
    ///     下载全部或指定服务器的服务器助力信息到缓存中。
    /// </summary>
    /// <param name="guilds"> 要下载服务器助力信息的服务器。如果为 <c>null</c>，则下载所有可用的服务器。 </param>
    /// <remarks>
    ///     对于要下载服务器助力信息的服务器，当前用户在该服务器中必须具有 <see cref="Kook.GuildPermission.ManageGuild"/> 权限。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public abstract Task DownloadBoostSubscriptionsAsync(IEnumerable<IGuild>? guilds = null,
        RequestOptions? options = null);

    /// <inheritdoc />
    Task<IChannel?> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IChannel?>(GetChannel(id));

    /// <inheritdoc />
    Task<IDMChannel?> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IDMChannel?>(GetDMChannel(chatCode));

    /// <inheritdoc />
    Task<IGuild?> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuild?>(GetGuild(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

    /// <inheritdoc />
    async Task<IUser?> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (GetUser(id) is { } user)
            return user;
        if (mode == CacheMode.CacheOnly)
            return null;
        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    Task<IUser?> IKookClient.GetUserAsync(string username, string identifyNumber, RequestOptions? options) =>
        Task.FromResult<IUser?>(GetUser(username, identifyNumber));
}
