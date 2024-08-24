namespace Kook;

/// <summary>
///     表示一个消息中解析出的哔哩哔哩视频嵌入式内容。
/// </summary>
public struct BilibiliVideoEmbed : IEmbed
{
    internal BilibiliVideoEmbed(string url, string originUrl, string bvNumber, string iframePath, TimeSpan duration, string title, string cover)
    {
        Url = url;
        OriginUrl = originUrl;
        BvNumber = bvNumber;
        IframePath = iframePath;
        Duration = duration;
        Title = title;
        Cover = cover;
    }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.BilibiliVideo;

    /// <summary>
    ///     获取视频所在页面的 URL。
    /// </summary>
    public string Url { get; }

    /// <summary>
    ///     获取嵌入式内容所解析的原始 URL。
    /// </summary>
    public string OriginUrl { get; }

    /// <summary>
    ///     获取视频的 BV 号。
    /// </summary>
    public string BvNumber { get; }

    /// <summary>
    ///     获取视频的 iframe 路径。
    /// </summary>
    public string IframePath { get; }

    /// <summary>
    ///     获取视频的时长。
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    ///     获取视频的标题。
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     获取图像封面图像的 URL。
    /// </summary>
    public string Cover { get; }
}
