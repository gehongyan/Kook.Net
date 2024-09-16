namespace Kook;

/// <summary>
///     表示 <see cref="Kook.IVoiceChannel"/> 的语音质量。
/// </summary>
public enum VoiceQuality
{
    /// <summary>
    ///     等效于约 18 kbps。
    /// </summary>
    _18kbps = 1,

    /// <summary>
    ///     等效于约 48 kbps。
    /// </summary>
    _48kbps = 2,

    /// <summary>
    ///     等效于约 96 kbps。
    /// </summary>
    _96kbps = 3,

    /// <summary>
    ///     等效于约 128 kbps。
    /// </summary>
    /// <remarks>
    ///     此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level1"/>。
    /// </remarks>
    _128kbps = 4,

    /// <summary>
    ///     等效于约 192 kbps。
    /// </summary>
    /// <remarks>
    ///     此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level2"/>。
    /// </remarks>
    _192kbps = 5,

    /// <summary>
    ///     等效于约 256 kbps。
    /// </summary>
    /// <remarks>
    ///     此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level3"/>。
    /// </remarks>
    _256kbps = 6,

    /// <summary>
    ///     等效于约 320 kbps。
    /// </summary>
    /// <remarks>
    ///     此质量需要服务器助力等级达到 <see cref="Kook.BoostLevel.Level5"/>。
    /// </remarks>
    _320kbps = 7
}
