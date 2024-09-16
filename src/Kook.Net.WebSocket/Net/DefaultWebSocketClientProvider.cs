using System.Net;

namespace Kook.Net.WebSockets;

/// <summary>
///     表示一个默认的 <see cref="Kook.Net.WebSockets.WebSocketProvider"/>，用于创建
///     <see cref="Kook.Net.WebSockets.IWebSocketClient"/> 的默认实现的实例。
/// </summary>
public static class DefaultWebSocketProvider
{
    /// <summary>
    ///     获取一个默认的 <see cref="Kook.Net.WebSockets.WebSocketProvider"/> 委托，用于创建
    ///     <see cref="Kook.Net.WebSockets.IWebSocketClient"/> 的默认实现的实例。
    /// </summary>
    public static readonly WebSocketProvider Instance = Create();

    /// <summary>
    ///     创建一个新的 <see cref="Kook.Net.WebSockets.WebSocketProvider"/> 委托。
    /// </summary>
    /// <param name="useProxy"> 是否使用系统代理。 </param>
    /// <returns> 一个新的 <see cref="Kook.Net.WebSockets.WebSocketProvider"/> 委托。 </returns>
    public static WebSocketProvider Create(IWebProxy? useProxy = null) =>
        () =>
        {
            try
            {
                return new DefaultWebSocketClient(useProxy);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.", ex);
            }
        };
}
