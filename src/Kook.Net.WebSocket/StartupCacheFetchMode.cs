namespace Kook.WebSocket;

/// <summary>
///     表示基于网关的客户端在启动时缓存基础数据的模式。
/// </summary>
/// <remarks>
///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
/// </remarks>
public enum StartupCacheFetchMode
{
    /// <summary>
    ///     根据服务器数量自动选择最佳模式。
    /// </summary>
    /// <remarks>
    ///     如果服务器数量达到了 <see cref="Kook.WebSocket.KookSocketConfig.LargeNumberOfGuildsThreshold"/>
    ///     的值，模式将为 <see cref="Kook.WebSocket.StartupCacheFetchMode.Lazy"/>；否则，如果达到了
    ///     <see cref="Kook.WebSocket.KookSocketConfig.SmallNumberOfGuildsThreshold"/>
    ///     的值，模式将为 <see cref="Kook.WebSocket.StartupCacheFetchMode.Asynchronous"/>；
    ///     否则，模式将为 <see cref="Kook.WebSocket.StartupCacheFetchMode.Synchronous"/>。
    /// </remarks>
    Auto,

    /// <summary>
    ///     同步主动获取。
    /// </summary>
    /// <remarks>
    ///     当客户端启动时，将主动通过 KOOK API 获取所有服务器基础数据，数据获取完成后引发
    ///     <see cref="Kook.WebSocket.KookSocketClient.Ready"/> 事件。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    /// </remarks>
    Synchronous,

    /// <summary>
    ///     异步主动获取。
    /// </summary>
    /// <remarks>
    ///     当客户端启动时，将尽快引发 <see cref="Kook.WebSocket.KookSocketClient.Ready"/> 事件，然后启动一个后台任务，通过
    ///     KOOK API 获取所有服务器基础数据。如果在基础数据获取期间接收到与服务器相关的事件，但该服务器的基础数据尚未获取，
    ///     事件处理程序将在引发用户代码订阅的事件之前获取该服务器的基础数据。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    /// </remarks>
    Asynchronous,

    /// <summary>
    ///     被动获取。
    /// </summary>
    /// <remarks>
    ///     当客户端启动时，将尽快引发 <see cref="Kook.WebSocket.KookSocketClient.Ready"/>
    ///     事件。当接收到与服务器相关的事件，但该服务器的基础数据尚未获取时，事件处理程序将在引发用户代码订阅的事件之前获取该服务器的基础数据。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    /// </remarks>
    Lazy
}
