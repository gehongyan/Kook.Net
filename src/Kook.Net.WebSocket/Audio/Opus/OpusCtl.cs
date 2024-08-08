namespace Kook.Audio;

/// <summary>
///     表示 Opus 控制码。
/// </summary>
/// <remarks>
///     参见 https://github.com/gcp/opus/blob/master/include/opus_defines.h
/// </remarks>
internal enum OpusCtl
{
    /// <summary>
    ///     <c>OPUS_SET_BITRATE</c>
    /// </summary>
    /// <remarks>
    ///     配置编码器中的比特率。
    /// </remarks>
    SetBitrate = 4002,

    /// <summary>
    ///     <c>OPUS_SET_BANDWIDTH_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     配置编码器中的最大带宽。
    /// </remarks>
    SetBandwidth = 4008,

    /// <summary>
    ///     <c>OPUS_SET_PACKET_LOSS_PERC_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     配置编码器中预期的丢包百分比。
    /// </remarks>
    SetPacketLossPercent = 4014,

    /// <summary>
    ///     <c>OPUS_SET_SIGNAL_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     配置正在编码的信号类型。
    /// </remarks>
    SetSignal = 4024
}
