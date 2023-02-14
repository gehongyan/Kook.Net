namespace Kook;

/// <summary>
///     Specifies the voice quality of an <see cref="IVoiceChannel"/>.
/// </summary>
public enum VoiceQuality
{
    /// <summary>
    ///     Equivalent to approximately 18 kbps.
    /// </summary>
    _18kbps = 1,
    /// <summary>
    ///     Equivalent to approximately is 48 kbps.
    /// </summary>
    _48kbps = 2,
    /// <summary>
    ///     Equivalent to approximately is 96 kbps.
    /// </summary>
    _96kbps = 3,
    /// <summary>
    ///     Equivalent to approximately 128 kbps.
    /// </summary>
    /// <remarks>
    ///     This quality needs the boost level to reach LV1. 
    /// </remarks>
    _128kbps = 4,
    /// <summary>
    ///     Equivalent to approximately 192 kbps.
    /// </summary>
    /// <remarks>
    ///     This quality needs the boost level to reach LV2. 
    /// </remarks>
    _192kbps = 5,
    /// <summary>
    ///     Equivalent to approximately 256 kbps.
    /// </summary>
    /// <remarks>
    ///     This quality needs the boost level to reach LV3. 
    /// </remarks>
    _256kbps = 6,
    /// <summary>
    ///     Equivalent to approximately 320 kbps.
    /// </summary>
    /// <remarks>
    ///     This quality needs the boost level to reach LV5. 
    /// </remarks>
    _320kbps = 7,
}
