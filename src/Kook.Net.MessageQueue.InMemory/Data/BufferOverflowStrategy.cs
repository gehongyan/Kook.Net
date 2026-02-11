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
    ///     立即处理缓冲中序号最小的一项，并将当前入队消息写入缓冲。
    /// </summary>
    ShiftOne,

    /// <summary>
    ///     抛出异常。
    /// </summary>
    ThrowException,

    /// <summary>
    ///     触发 <see cref="BaseMessageQueue.ReconnectRequested"/> 通知网关线程重连。
    /// </summary>
    ReconnectGateway,
}
