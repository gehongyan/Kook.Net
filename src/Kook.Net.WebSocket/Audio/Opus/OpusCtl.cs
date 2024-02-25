namespace Kook.Audio;

/// <summary>
///     Represents the Opus control codes.
/// </summary>
/// <remarks>
///     See https://github.com/gcp/opus/blob/master/include/opus_defines.h
/// </remarks>
internal enum OpusCtl
{
    /// <summary>
    ///     <c>OPUS_SET_BITRATE</c>
    /// </summary>
    /// <remarks>
    ///     Configures the bitrate in the encoder.
    /// </remarks>
    SetBitrate = 4002,

    /// <summary>
    ///     <c>OPUS_SET_BANDWIDTH_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     Configures the maximum bandwidth in the encoder.
    /// </remarks>
    SetBandwidth = 4008,

    /// <summary>
    ///     <c>OPUS_SET_PACKET_LOSS_PERC_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     Configures the expected packet loss percentage in the encoder.
    /// </remarks>
    SetPacketLossPercent = 4014,

    /// <summary>
    ///     <c>OPUS_SET_SIGNAL_REQUEST</c>
    /// </summary>
    /// <remarks>
    ///     Configures the type of signal being encoded.
    /// </remarks>
    SetSignal = 4024
}
