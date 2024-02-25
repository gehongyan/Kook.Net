namespace Kook.Audio;

/// <summary>
///     Represents the signal type for Opus.
/// </summary>
internal enum OpusSignal
{
    /// <summary>
    ///     <c>OPUS_SIGNAL_AUTO</c>
    /// </summary>
    /// <remarks>
    ///     Signal type is automatically detected.
    /// </remarks>
    Auto = -1000,

    /// <summary>
    ///     <c>OPUS_SIGNAL_VOICE</c>
    /// </summary>
    /// <remarks>
    ///     Signal being encoded is voice.
    /// </remarks>
    Voice = 3001,

    /// <summary>
    ///     <c>OPUS_SIGNAL_MUSIC</c>
    /// </summary>
    /// <remarks>
    ///     Signal being encoded is music.
    /// </remarks>
    Music = 3002,
}
