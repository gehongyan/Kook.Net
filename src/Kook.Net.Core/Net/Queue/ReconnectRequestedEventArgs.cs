namespace Kook.Net.Queue;

/// <summary>
///     表示消息队列请求网关重连时的事件参数。
/// </summary>
public class ReconnectRequestedEventArgs : EventArgs
{
    /// <summary>
    ///     初始化 <see cref="ReconnectRequestedEventArgs"/> 的新实例。
    /// </summary>
    /// <param name="reason"> 请求重连的原因。 </param>
    /// <param name="exception"> 描述原因的异常，将用于触发网关重连。 </param>
    public ReconnectRequestedEventArgs(ReconnectRequestedReason reason, Exception exception)
    {
        Reason = reason;
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary>
    ///     获取请求重连的原因。
    /// </summary>
    public ReconnectRequestedReason Reason { get; }

    /// <summary>
    ///     获取描述原因的异常，由网关线程用于执行重连（如传入 <c>Connection.Error</c>）。
    /// </summary>
    public Exception Exception { get; }
}
