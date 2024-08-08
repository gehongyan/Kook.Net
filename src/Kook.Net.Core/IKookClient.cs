namespace Kook;

/// <summary>
///     表示一个通用的 KOOK 客户端。
/// </summary>
public interface IKookClient : IDisposable
{
    #region General

    /// <summary>
    ///     获取当前连接的状态。
    /// </summary>
    ConnectionState ConnectionState { get; }

    /// <summary>
    ///     获取当前已登录的用户；如果没有用户登录，则为 <c>null</c>。
    /// </summary>
    ISelfUser? CurrentUser { get; }

    /// <summary>
    ///     获取已登录用户的令牌类型。
    /// </summary>
    TokenType TokenType { get; }

    /// <summary>
    ///     启动客户端与 KOOK 之间的连接。
    /// </summary>
    /// <remarks>
    ///     当前方法会初始化客户端与 KOOK 之间的连接。 <br />
    ///     <note type="important">
    ///         此方法会在调用后立即返回，因为它会在另一个线程上初始化连接。
    ///     </note>
    /// </remarks>
    /// <returns> 一个表示异步启动操作的任务。 </returns>
    Task StartAsync();

    /// <summary>
    ///     停止客户端与 KOOK 之间的连接。
    /// </summary>
    /// <returns> 一个表示异步停止操作的任务。 </returns>
    Task StopAsync();

    #endregion

    #region Channels

    /// <summary>
    ///     获取一个频道。
    /// </summary>
    /// <param name="id"> 频道的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的频道；若指定 ID 的频道不存在，则为 <c>null</c>。 </returns>
    Task<IChannel?> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取一个私聊频道。
    /// </summary>
    /// <param name="chatCode"> 私聊频道的聊天代码。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定聊天代码的私聊频道；若指定聊天代码的私聊频道不存在，则为 <c>null</c>。 </returns>
    Task<IDMChannel?> GetDMChannelAsync(Guid chatCode, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取当前会话中已创建的所有私聊频道。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         此方法不会返回当前会话之外已创建的私聊频道。如果客户端刚刚启动，这可能会返回一个空集合。
    ///     </note>
    /// </remarks>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是当前会话中已创建的所有私聊频道。 </returns>
    Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Guilds

    /// <summary>
    ///     获取一个服务器。
    /// </summary>
    /// <param name="id"> 服务器的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的服务器；若指定 ID 的服务器不存在，则为 <c>null</c>。 </returns>
    Task<IGuild?> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取当前用户所在的所有服务器。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是当前用户所在的所有服务器。 </returns>
    Task<IReadOnlyCollection<IGuild>> GetGuildsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Users

    /// <summary>
    ///     获取一个用户。
    /// </summary>
    /// <param name="id"> 用户的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的用户；若指定 ID 的用户不存在，则为 <c>null</c>。 </returns>
    Task<IUser?> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取一个用户。
    /// </summary>
    /// <param name="username"> 用户的名称。 </param>
    /// <param name="identifyNumber"> 用户的识别号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定的名称和识别号的用户；如果未找到该用户，则为 <c>null</c>。 </returns>
    Task<IUser?> GetUserAsync(string username, string identifyNumber, RequestOptions? options = null);

    #endregion

    #region Friends

    /// <summary>
    ///     获取所有好友。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有与当前用户是好友的用户。 </returns>
    Task<IReadOnlyCollection<IUser>> GetFriendsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取所有好友请求。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有请求与当前用户成为好友的用户。 </returns>
    Task<IReadOnlyCollection<IFriendRequest>> GetFriendRequestsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取所有被当前用户屏蔽的用户。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有被当前用户屏蔽的用户。 </returns>
    Task<IReadOnlyCollection<IUser>> GetBlockedUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion
}
