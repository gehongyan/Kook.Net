namespace Kook;

/// <summary>
///     指定客户端的连接状态。
/// </summary>
public enum ConnectionState : byte
{
    /// <summary>
    ///     客户端已断开与 KOOK 的连接。
    /// </summary>
    Disconnected,

    /// <summary>
    ///     客户端正在连接到 KOOK。
    /// </summary>
    Connecting,

    /// <summary>
    ///     客户端已连接到 KOOK。
    /// </summary>
    Connected,

    /// <summary>
    ///     客户端正在断开与 KOOK 的连接。
    /// </summary>
    Disconnecting
}
