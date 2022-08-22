namespace Kook;

/// <summary>
///     Represents a link embed.
/// </summary>
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
    
    /// <inheritdoc />
    public EmbedType Type => EmbedType.Link;
    
    /// <inheritdoc />
    public string Url { get; internal set; }

    /// <summary>
    ///     Gets the title of the website the link directs to.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the title of the website the link directs to.
    /// </returns>
    public string Title { get; internal set; }

    /// <summary>
    ///     Gets the description of the website the link directs to.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the description of the website the link directs to.
    /// </returns>
    public string Description { get; internal set; }

    /// <summary>
    ///     Gets the name of the website the link directs to.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the name of the website the link directs to.
    /// </returns>
    public string SiteName { get; internal set; }

    /// <summary>
    ///     Gets the color displayed along the left side of the card.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the color displayed along the left side of the card.
    /// </returns>
    public Color Color { get; internal set; }

    /// <summary>
    ///     Gets the URL of the image related to the website the link directs to.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the URL of the image related to the website the link directs to.
    /// </returns>
    public string Image { get; internal set; }
}