namespace Kook.Net.Queue.InMemory;

/// <summary>
///     带缓冲的内存消息队列的配置选项。
/// </summary>
public class InMemoryMessageQueueOptions
{
    /// <summary>
    ///     获取或设置是否启用缓冲区。
    /// </summary>
    public bool EnableBuffering { get; set; } = false;

    /// <summary>
    ///     序号上限（含），用于序号回绕计算。有效序号范围为 [0, <see cref="MaxSequenceNumber"/>]。默认
    ///     <see cref="ushort.MaxValue"/>，最小 <c>1</c>。
    /// </summary>
    public int MaxSequenceNumber
    {
        get;
        set
        {
            Preconditions.AtLeast(
                value, 1, nameof(MaxSequenceNumber),
                $"{nameof(MaxSequenceNumber)} is required to be non-negative.");
            field = value;
        }
    } = ushort.MaxValue;

    /// <summary>
    ///     缓冲最大条数。当缓冲已满且新消息需要入缓冲时，按 <see cref="BufferOverflowStrategy"/> 处理。默认
    ///     <c>50</c>，最小 <c>1</c>。
    /// </summary>
    public int BufferCapacity
    {
        get;
        set
        {
            Preconditions.AtLeast(
                value, 1, nameof(BufferCapacity),
                $"{nameof(BufferCapacity)} is required to be at least 1.");
            field = value;
        }
    } = 50;

    /// <summary>
    ///     缓冲溢出时的处理策略。
    /// </summary>
    public BufferOverflowStrategy BufferOverflowStrategy { get; set; } = BufferOverflowStrategy.DropIncoming;

    /// <summary>
    ///     等待网关补齐缺失序号的最大时长。
    /// </summary>
    /// <remarks>
    ///     为 <c>null</c> 或 <see cref="Timeout.InfiniteTimeSpan"/> 表示不启用超时，一直等待缺失序号直至到达。
    /// </remarks>
    public TimeSpan? WaitForMissingTimeout { get; set; } = Timeout.InfiniteTimeSpan;

    /// <summary>
    ///     等待缺失序号超时后的处理策略。
    /// </summary>
    public BufferWaitTimeoutStrategy BufferWaitTimeoutStrategy { get; set; } =
        BufferWaitTimeoutStrategy.SkipMissing;
}
