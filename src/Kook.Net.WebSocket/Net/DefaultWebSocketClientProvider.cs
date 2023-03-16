using System.Net;

namespace Kook.Net.WebSockets;

/// <summary>
///     Represents a default <see cref="WebSocketProvider"/> that creates <see cref="DefaultWebSocketClient"/> instances.
/// </summary>
public static class DefaultWebSocketProvider
{
    /// <summary>
    ///     The default <see cref="WebSocketProvider"/> instance.
    /// </summary>
    public static readonly WebSocketProvider Instance = Create();

    /// <summary>
    ///     Creates a delegate that creates a new <see cref="DefaultWebSocketClient"/> instance.
    /// </summary>
    /// <param name="proxy"> The proxy to use. </param>
    /// <returns> A delegate that creates a new <see cref="DefaultWebSocketClient"/> instance. </returns>
    /// <exception cref="PlatformNotSupportedException">The default WebSocketProvider is not supported on this platform.</exception>
    public static WebSocketProvider Create(IWebProxy proxy = null)
    {
        return () =>
        {
            try
            {
                return new DefaultWebSocketClient(proxy);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default WebSocketProvider is not supported on this platform.", ex);
            }
        };
    }
}
