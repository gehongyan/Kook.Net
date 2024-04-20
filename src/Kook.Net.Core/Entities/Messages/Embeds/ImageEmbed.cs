namespace Kook;

/// <summary>
///     Represents an image embed.
/// </summary>
public struct ImageEmbed : IEmbed
{
    internal ImageEmbed(string url, string originUrl)
    {
        Url = url;
        OriginUrl = originUrl;
    }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.Link;

    /// <summary>
    ///     Gets the URL of this embed.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> that represents the URL of this embed.
    /// </returns>
    public string Url { get; internal set; }

    /// <summary>
    ///     Gets the original URL of the image.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the original URL of the image.
    /// </returns>
    public string OriginUrl { get; internal set; }
}
