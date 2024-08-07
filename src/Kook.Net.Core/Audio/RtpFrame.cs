namespace Kook.Audio;

/// <summary>
///     表示一个 RTP 帧。
/// </summary>
public readonly struct RtpFrame
{
    /// <summary>
    ///     获取此帧的序列号。
    /// </summary>
    public ushort Sequence { get; init; }

    /// <summary>
    ///     获取此帧的时间戳。
    /// </summary>
    public uint Timestamp { get; init; }

    /// <summary>
    ///     获取此帧的有效负载。
    /// </summary>
    public byte[] Payload { get; init; }

    /// <summary>
    ///     获取此帧是否丢失。
    /// </summary>
    public bool Missed { get; init; }

    /// <summary>
    ///     初始化 <see cref="RtpFrame"/> 结构的新实例。
    /// </summary>
    /// <param name="sequence"> 序列号。 </param>
    /// <param name="timestamp"> 时间戳。 </param>
    /// <param name="payload"> 有效负载。 </param>
    /// <param name="missed"> 是否丢失。 </param>
    public RtpFrame(ushort sequence, uint timestamp, byte[] payload, bool missed)
    {
        Sequence = sequence;
        Timestamp = timestamp;
        Payload = payload;
        Missed = missed;
    }
}
