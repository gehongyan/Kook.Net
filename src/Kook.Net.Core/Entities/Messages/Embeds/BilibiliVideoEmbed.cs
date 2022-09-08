namespace Kook;

public struct BilibiliVideoEmbed : IEmbed
{
    public BilibiliVideoEmbed(string url, string originUrl, string bvNumber, string iframePath, TimeSpan duration, string title, string cover)
    {
        Url = url;
        OriginUrl = originUrl;
        BvNumber = bvNumber;
        IframePath = iframePath;
        Duration = duration;
        Title = title;
        Cover = cover;
    }
    
    public EmbedType Type => EmbedType.Link;
    
    public string Url { get; internal set; }

    public string OriginUrl { get; internal set; }

    public string BvNumber { get; internal set; }

    public string IframePath { get; internal set; }

    public TimeSpan Duration { get; internal set; }

    public string Title { get; internal set; }

    public string Cover { get; internal set; }
}