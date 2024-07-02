namespace Kook.Audio;

/// <summary>
///     表示 Opus 信号类型。
/// </summary>
internal enum OpusSignal
{
    /// <summary>
    ///     <c>OPUS_SIGNAL_AUTO</c>
    /// </summary>
    /// <remarks>
    ///     自动检测信号类型。
    /// </remarks>
    Auto = -1000,

    /// <summary>
    ///     <c>OPUS_SIGNAL_VOICE</c>
    /// </summary>
    /// <remarks>
    ///     被编码的信号是语音。
    /// </remarks>
    Voice = 3001,

    /// <summary>
    ///     <c>OPUS_SIGNAL_MUSIC</c>
    /// </summary>
    /// <remarks>
    ///     被编码的信号是音乐。
    /// </remarks>
    Music = 3002
}
