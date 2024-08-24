namespace Kook.Audio;

/// <summary>
///     表示一个通用的输入音频流。
/// </summary>
public abstract class AudioInStream : AudioStream
{
    /// <summary>
    ///     获取当前可用的帧数。
    /// </summary>
    public abstract int AvailableFrames { get; }

    /// <inheritdoc />
    public override bool CanRead => true;
    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <summary>
    ///     读取一个 RTP 帧。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    /// <returns> 一个表示异步读取的任务。任务的结果为读取到的 RTP 帧。 </returns>
    public abstract Task<RtpFrame> ReadFrameAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     尝试读取一个 RTP 帧。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    /// <param name="frame"> 如果成功读取到 RTP 帧，则为读取到的 RTP 帧；否则为 <see langword="default"/>。 </param>
    /// <returns> 是否成功读取到 RTP 帧。 </returns>
    public abstract bool TryReadFrame(CancellationToken cancellationToken, out RtpFrame frame);

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) => throw new NotSupportedException();
}
