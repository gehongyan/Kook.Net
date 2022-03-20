namespace KaiHeiLa;

public class LinkEmbed : IEmbed
{
    internal LinkEmbed(string url, string title, string description, string siteName, Color color, string image)
    {
        Url = url;
        Title = title;
        Description = description;
        SiteName = siteName;
        Color = color;
        Image = image;
    }
    
    public EmbedType Type => EmbedType.Link;
    
    public string Url { get; internal set; }

    public string Title { get; internal set; }

    public string Description { get; internal set; }

    public string SiteName { get; internal set; }

    public Color Color { get; internal set; }

    public string Image { get; internal set; }
}