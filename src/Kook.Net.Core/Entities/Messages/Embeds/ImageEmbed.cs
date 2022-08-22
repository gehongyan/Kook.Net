namespace Kook;

/// <summary>
///     Represents an image embed.
/// </summary>
public class ImageEmbed : IEmbed
{
    internal ImageEmbed(string url, string originUrl)
    {
        Url = url;
        OriginUrl = originUrl;
    }
    
    /// <inheritdoc />
    public EmbedType Type => EmbedType.Link;
    
    /// <inheritdoc />
    public string Url { get; internal set; }

    /// <summary>
    ///     Gets the original URL of the image.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> representing the original URL of the image.
    /// </returns>
    public string OriginUrl { get; internal set; }
}