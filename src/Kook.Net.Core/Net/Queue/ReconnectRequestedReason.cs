namespace Kook.Net.Queue;

/// <summary>
///     表示消息队列请求网关重连的原因。
/// </summary>
public enum ReconnectRequestedReason
{
    /// <summary>
    ///     按序缓冲已满，无法继续接收乱序消息。
    /// </summary>
    BufferOverflow,

    /// <summary>
    ///     等待缺失序号超时，建议重连以重新同步序号。
    /// </summary>
    WaitTimeout,
}
