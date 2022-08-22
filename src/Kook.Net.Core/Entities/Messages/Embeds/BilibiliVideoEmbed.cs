namespace Kook;

public class BilibiliVideoEmbed : IEmbed
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

    public string BvNumber { get; set; }

    public string IframePath { get; set; }

    public TimeSpan Duration { get; set; }

    public string Title { get; set; }

    public string Cover { get; set; }
}