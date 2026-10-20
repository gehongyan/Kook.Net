namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     点歌队列中的一首歌。
/// </summary>
/// <param name="Title"> 显示用的标题。 </param>
/// <param name="Source"> 交给 ffmpeg 的音源：http(s) 链接或本地文件路径。 </param>
public sealed record MusicTrack(string Title, string Source)
{
    /// <summary> 音源是否是网络链接。 </summary>
    public bool IsRemote => Uri.TryCreate(Source, UriKind.Absolute, out Uri? uri) && uri.Scheme is "http" or "https";
}
