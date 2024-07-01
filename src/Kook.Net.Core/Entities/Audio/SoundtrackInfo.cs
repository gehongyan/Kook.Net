namespace Kook.Audio;

/// <summary>
///     表示计算机音频共享信息。
/// </summary>
public readonly struct SoundtrackInfo
{
    /// <summary>
    ///     获取计算机音频来源的应用程序名称。
    /// </summary>
    public string? Software { get; init; }

    /// <summary>
    ///     获取计算机音频共享的音乐名称。
    /// </summary>
    public string? Music { get; init; }

    /// <summary>
    ///     获取计算机音频共享的音乐的歌手名称。
    /// </summary>
    public string? Singer { get; init; }
}
