namespace Kook.Audio;

/// <summary>
///     Represents the application type for Opus encoding.
/// </summary>
internal enum OpusApplication
{
    /// <summary>
    ///     <c>OPUS_APPLICATION_VOIP</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_VOIP</c> gives best quality at a given bitrate for voice signals.
    ///     It enhances the input signal by high-pass filtering and emphasizing formants and harmonics.
    ///     Optionally it includes in-band forward error correction to protect against packet loss.
    ///     Use this mode for typical VoIP applications. Because of the enhancement,
    ///     even at high bitrates the output may sound different from the input.
    /// </remarks>
    Voice = 2048,

    /// <summary>
    ///     <c>OPUS_APPLICATION_AUDIO</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_AUDIO</c> gives best quality at a given bitrate for most non-voice signals like music.
    ///     Use this mode for music and mixed (music/voice) content, broadcast, and applications
    ///     requiring less than 15 ms of coding delay.
    /// </remarks>
    MusicOrMixed = 2049,

    /// <summary>
    ///     <c>OPUS_APPLICATION_RESTRICTED_LOWDELAY</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_RESTRICTED_LOWDELAY</c> configures low-delay mode that disables
    ///     the speech-optimized mode in exchange for slightly reduced delay.
    /// </remarks>
    LowLatency = 2051
}
