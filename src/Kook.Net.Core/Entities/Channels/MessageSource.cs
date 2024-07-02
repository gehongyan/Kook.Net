namespace Kook;

/// <summary>
///     表示消息的来源。
/// </summary>
public enum MessageSource
{
    /// <summary>
    ///     系统消息。
    /// </summary>
    System,

    /// <summary>
    ///     消息由用户发送。
    /// </summary>
    User,

    /// <summary>
    ///     消息由 Bot 发送。
    /// </summary>
    Bot
}
