namespace Kook;

/// <summary>
///     表示 Kook.Net 调试器消息的来源。
/// </summary>
public enum KookDebuggerMessageSource
{
    /// <summary>
    ///     表示来自 Rest 调试器。
    /// </summary>
    Rest,

    /// <summary>
    ///     表示来自网关数据包调试器。
    /// </summary>
    Packet,

    /// <summary>
    ///     表示来自速率限制调试器。
    /// </summary>
    Ratelimit,

    /// <summary>
    ///     表示来自音频调试器。
    /// </summary>
    Audio
}
