namespace KaiHeiLa;

/// <summary>
///     Specifies the voice quality of an <see cref="IVoiceChannel"/>.
/// </summary>
public enum VoiceQuality
{
    /// <summary>
    ///     The voice quality is unknown.
    /// </summary>
    Unspecified = 0,
    /// <summary>
    ///     The voice quality is 18 Kbps.
    /// </summary>
    Fluent = 1,
    /// <summary>
    ///     The voice quality is 48 Kbps.
    /// </summary>
    Normal = 2,
    /// <summary>
    ///     The voice quality is 96 Kbps.
    /// </summary>
    High = 3,
    /// <summary>
    ///     The voice quality is 128 Kbps.
    /// </summary>
    Higher = 4
}