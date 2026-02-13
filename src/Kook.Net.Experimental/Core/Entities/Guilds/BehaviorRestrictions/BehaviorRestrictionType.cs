namespace Kook;

/// <summary>
///     表示服务器行为限制的类型。
/// </summary>
public enum BehaviorRestrictionType
{
    /// <summary>
    ///     消息操作行为限制。
    /// </summary>
    MessageOperation = 1,

    /// <summary>
    ///     语音连接行为限制。
    /// </summary>
    VoiceConnection = 2,

    /// <summary>
    ///     消息操作和语音连接行为限制。
    /// </summary>
    MessageOperationAndVoiceConnection = 3,
}
