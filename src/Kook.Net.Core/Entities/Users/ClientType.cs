namespace Kook;

/// <summary>
///     表示一个用户所登录的客户端类型。
/// </summary>
public enum ClientType
{
    /// <summary>
    ///     用户正在使用 WebSocket 连接到服务器。
    /// </summary>
    WebSocket,

    /// <summary>
    ///     用户正在使用 Android 应用程序连接到服务器。
    /// </summary>
    Android,

    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     用户正在使用 iOS 应用程序连接到服务器。
    /// </summary>
    iOS
}
