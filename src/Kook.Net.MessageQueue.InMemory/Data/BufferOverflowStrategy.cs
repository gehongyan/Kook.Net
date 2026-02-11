namespace Kook.Net.Queue.InMemory;

/// <summary>
///     表示当按序缓冲队列已满且收到需要入缓冲的新消息时的处理策略。
/// </summary>
public enum BufferOverflowStrategy
{
    /// <summary>
    ///     丢弃当前入队消息。
    /// </summary>
    DropIncoming,

    /// <summary>
    ///     忽略缺失序号，将下一期望序号推进到当前缓冲中的最小序号，并顺序处理缓冲中的连续消息。
    /// </summary>
    SkipMissing,

    /// <summary>
    ///     抛出异常。
    /// </summary>
    ThrowException,

    /// <summary>
    ///     触发 <see cref="BaseMessageQueue.ReconnectRequested"/> 通知网关线程重连。
    /// </summary>
    ReconnectGateway,
}
