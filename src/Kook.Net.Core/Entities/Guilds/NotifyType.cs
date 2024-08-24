namespace Kook;

/// <summary>
///     表示服务器应如何通知用户。
/// </summary>
public enum NotifyType
{
    /// <summary>
    ///     以服务器的默认行为通知用户。
    /// </summary>
    Default = 0,

    /// <summary>
    ///     通知用户所有消息。
    /// </summary>
    AcceptAll = 1,

    /// <summary>
    ///     通知提及用户的消息。
    /// </summary>
    OnlyMentioned = 2,

    /// <summary>
    ///     从不通知用户。
    /// </summary>
    Muted = 3
}
