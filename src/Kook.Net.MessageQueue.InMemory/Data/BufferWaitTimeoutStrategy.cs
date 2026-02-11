namespace Kook.Net.Queue.InMemory;

/// <summary>
///     表示在等待缺失序号超时后的处理策略。
/// </summary>
public enum BufferWaitTimeoutStrategy
{
    /// <summary>
    ///     跳过缺失序号，将下一期望序号推进到缓冲中的最小序号，并顺序处理缓冲中的连续消息。
    /// </summary>
    SkipMissing,

    /// <summary>
    ///     触发 <see cref="BaseMessageQueue.ReconnectRequested"/> 通知网关线程重连。
    /// </summary>
    ReconnectGateway,
}
