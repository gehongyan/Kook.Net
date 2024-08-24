namespace Kook;

/// <summary>
///     表示音乐的信息。
/// </summary>
public class Music
{
    /// <summary>
    ///     获取或设置音乐的提供来源。
    /// </summary>
    public MusicProvider Provider { get; set; }

    /// <summary>
    ///     获取或设置音乐的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置音乐的歌手名称。
    /// </summary>
    public string? Singer { get; set; }
}
