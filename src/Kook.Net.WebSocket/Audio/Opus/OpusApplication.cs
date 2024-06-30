namespace Kook.Audio;

/// <summary>
///     表示 Opus 编码的应用程序类型。
/// </summary>
internal enum OpusApplication
{
    /// <summary>
    ///     <c>OPUS_APPLICATION_VOIP</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_VOIP</c> 在给定比特率下针对语音信号提供最佳质量。
    ///     它通过高通滤波、强调共振峰和谐波来增强输入信号，可以包括带内前向纠错以防止丢包。
    ///     在典型的 VoIP 应用中应使用此模式。由于增强效果，即使在高比特率下，输出的声音也可能与输入不同。
    /// </remarks>
    Voice = 2048,

    /// <summary>
    ///     <c>OPUS_APPLICATION_AUDIO</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_AUDIO</c> 在给定比特率下针对大多数非语音信号（如音乐）提供最佳质量。
    ///     适用于音乐、混合（音乐/语音）内容、广播以及需要少于 15 毫秒编码延迟的应用程序。
    /// </remarks>
    MusicOrMixed = 2049,

    /// <summary>
    ///     <c>OPUS_APPLICATION_RESTRICTED_LOWDELAY</c>
    /// </summary>
    /// <remarks>
    ///     <c>OPUS_APPLICATION_RESTRICTED_LOWDELAY</c> 为低延迟模式，禁用了针对语音优化的模式，来换取稍微减少的延迟。
    /// </remarks>
    LowLatency = 2051
}
