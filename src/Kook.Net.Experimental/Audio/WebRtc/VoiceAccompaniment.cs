namespace Kook.Audio.WebRtc;

/// <summary>
///     表示一个成员正在播放的伴奏的信息。
/// </summary>
/// <remarks>
///     所有属性都是可空的：服务端不保证每次都下发全部字段。
/// </remarks>
public sealed class VoiceAccompaniment
{
    /// <summary>
    ///     获取伴奏的歌曲名称。
    /// </summary>
    public string? MusicName { get; }

    /// <summary>
    ///     获取播放伴奏所使用的音乐软件。
    /// </summary>
    public string? MusicSoftware { get; }

    /// <summary>
    ///     获取伴奏的歌手。
    /// </summary>
    public string? Singer { get; }

    internal VoiceAccompaniment(string? musicName, string? musicSoftware, string? singer)
    {
        MusicName = musicName;
        MusicSoftware = musicSoftware;
        Singer = singer;
    }
}
