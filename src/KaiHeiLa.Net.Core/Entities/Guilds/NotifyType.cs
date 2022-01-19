namespace KaiHeiLa;

/// <summary>
///     服务器通知类型
/// </summary>
public enum NotifyType
{
    /// <summary>
    ///     服务器默认通知设置
    /// </summary>
    Default = 0,
    
    /// <summary>
    ///     接收所有通知
    /// </summary>
    AcceptAll = 1,
    
    /// <summary>
    ///     仅@被提及
    /// </summary>
    OnlyMentioned = 2,
    
    /// <summary>
    ///     不接收通知
    /// </summary>
    Muted = 3
}