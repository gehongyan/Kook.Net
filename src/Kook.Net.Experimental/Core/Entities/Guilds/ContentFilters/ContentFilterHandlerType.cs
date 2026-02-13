namespace Kook;

/// <summary>
///     表示一个内容过滤器的处理类型。
/// </summary>
public enum ContentFilterHandlerType
{
    /// <summary>
    ///     拦截消息。
    /// </summary>
    Intercept = 1,

    /// <summary>
    ///     发送拦截日志。
    /// </summary>
    LogToChannel = 2
}
