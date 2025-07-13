namespace Kook;

/// <summary>
///     表示 <see cref="Kook.IVoiceChannel"/> 的语音质量。
/// </summary>
public enum VoiceQuality
{
    /// <summary>
    ///     流畅。
    /// </summary>
    /// <remarks>
    ///     等效于约 18 kbps。
    /// </remarks>
    Smooth = 1,

    /// <inheritdoc cref="Smooth" />
    [Obsolete("Use VoiceQuality.Smooth instead.")]
    _18kbps = Smooth,

    /// <summary>
    ///     正常。
    /// </summary>
    /// <remarks>
    ///     等效于约 32 kbps。
    /// </remarks>
    Normal = 2,

    /// <inheritdoc cref="Normal" />
    [Obsolete("Use VoiceQuality.Normal instead.")]
    _48kbps = Normal,

    /// <summary>
    ///     质量。
    /// </summary>
    /// <remarks>
    ///     等效于约 64 kbps。
    /// </remarks>
    Standard = 3,

    /// <inheritdoc cref="Standard" />
    [Obsolete("Use VoiceQuality.Standard instead.")]
    _96kbps = Standard,

    /// <summary>
    ///     高质量。
    /// </summary>
    /// <remarks>
    ///     等效于约 128 kbps，此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level1"/>。
    /// </remarks>
    High = 4,

    /// <inheritdoc cref="High" />
    [Obsolete("Use VoiceQuality.High instead.")]
    _128kbps = High,

    /// <summary>
    ///     更高质量。
    /// </summary>
    /// <remarks>
    ///     等效于约 192 kbps，此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level2"/>。
    /// </remarks>
    Higher = 5,

    /// <inheritdoc cref="Higher" />
    [Obsolete("Use VoiceQuality.Higher instead.")]
    _192kbps = Higher,

    /// <summary>
    ///     卓越质量。
    /// </summary>
    /// <remarks>
    ///     等效于约 256 kbps，此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level3"/>。
    /// </remarks>
    Excellent = 6,

    /// <inheritdoc cref="Excellent" />
    [Obsolete("Use VoiceQuality.Excellent instead.")]
    _256kbps = Excellent,

    /// <summary>
    ///     极致质量。
    /// </summary>
    /// <remarks>
    ///     等效于约 320 kbps，此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level5"/>。
    /// </remarks>
    Ultimate = 7,

    /// <inheritdoc cref="Ultimate" />
    [Obsolete("Use VoiceQuality.Ultimate instead.")]
    _320kbps = Ultimate
}
