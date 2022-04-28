namespace KaiHeiLa.Utils;

internal static class UrlValidation
{
    /// <summary>
    ///     Not full URL validation right now. Just ensures protocol is present and that it's either http or https.
    /// </summary>
    /// <param name="url">The URL to validate before sending to KaiHeiLa.</param>
    /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
    /// <returns><c>true</c> if URL is valid by our standard, <c>false</c> if null, throws an error upon invalid.</returns>
    public static bool Validate(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;
        if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) 
              || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"The url {url} must include a protocol (either HTTP or HTTPS)");
        return true;
    }
}