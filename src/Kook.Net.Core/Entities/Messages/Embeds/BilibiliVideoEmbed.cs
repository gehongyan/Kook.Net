namespace Kook;

/// <summary>
///     Represents an embed in a message that links to a Bilibili video.
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
    public EmbedType Type => EmbedType.Link;

    /// <inheritdoc />
    public string Url { get; internal set; }

    /// <summary>
    ///     A <see langword="string"/> that represents the origin URL of the Bilibili video.
    /// </summary>
    public string OriginUrl { get; internal set; }

    /// <summary>
    ///     A <see langword="string"/> that represents the Bilibili video number in BV format.
    /// </summary>
    public string BvNumber { get; internal set; }

    /// <summary>
    ///     A <see langword="string"/> that represents the path of the iframe.
    /// </summary>
    public string IframePath { get; internal set; }

    /// <summary>
    ///     A <see cref="TimeSpan"/> that represents the duration of the Bilibili video.
    /// </summary>
    public TimeSpan Duration { get; internal set; }

    /// <summary>
    ///     A <see langword="string"/> that represents the title of the Bilibili video.
    /// </summary>
    public string Title { get; internal set; }

    /// <summary>
    ///     A <see langword="string"/> that represents the cover of the Bilibili video.
    /// </summary>
    public string Cover { get; internal set; }
}
