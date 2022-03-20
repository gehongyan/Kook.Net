namespace KaiHeiLa;

public class ImageEmbed : IEmbed
{
    public ImageEmbed(string url, string originUrl)
    {
        Url = url;
        OriginUrl = originUrl;
    }
    
    public EmbedType Type => EmbedType.Link;
    
    public string Url { get; internal set; }

    public string OriginUrl { get; internal set; }
}