using System.Text.RegularExpressions;

namespace Kook.Utils;

internal static class UrlValidation
{
    /// <summary>
    ///     Not full URL validation right now. Just ensures protocol is present and that it's either http or https.
    /// </summary>
    /// <param name="url">The URL to validate before sending to Kook.</param>
    /// <exception cref="UriFormatException">A URL cannot be null or empty.</exception>
    /// <exception cref="UriFormatException">A URL must include a protocol (http or https).</exception>
    /// <returns><c>true</c> if URL is valid by our standard, <c>false</c> if null, throws an error upon invalid.</returns>
    public static void Validate(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new UriFormatException("The URL cannot be null or empty.");

        if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            throw new UriFormatException($"The url {url} must include a protocol (either HTTP or HTTPS)");
    }

    /// <summary>
    ///     Ensures that the URL represents an asset on the Kook OSS server.
    /// </summary>
    /// <remarks>
    ///     This method checks the URL from 3 parts:
    ///     <list type="number">
    ///         <item>The scheme must be either http or https</item>
    ///         <item>
    ///             The host must be one of the following:
    ///             <list type="bullet">
    ///                 <item>img.kaiheila.cn</item>
    ///                 <item>img.kookapp.cn</item>
    ///                 <item>kaiheila.oss-cn-beijing.aliyuncs.com</item>
    ///             </list>
    ///         </item>
    ///         <item>The path must in the form of the following:
    ///             <list type="bullet">
    ///                 <item>/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg</item>
    ///                 <item>/assets/2022-01/21/HwuGGgpohG0ku0ku.jpeg</item>
    ///                 <item>/attachments/2022-08/19/GHUrXywopm1hc0u0.png</item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <param name="url">The URL to validate before sending to Kook.</param>
    /// <exception cref="InvalidOperationException">The URL provided is not an asset on the Kook OSS.</exception>
    /// <returns><c>true</c> if URL represents a valid asset on the Kook OSS, <c>false</c> if null, throws an error upon invalid.</returns>
    public static bool ValidateKookAssetUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;

        if (!Regex.IsMatch(url,
                @"^https?:\/\/(img\.(kaiheila|kookapp)\.cn|kaiheila\.oss-cn-beijing\.aliyuncs\.com)\/(assets|attachments)\/\d{4}-\d{2}(\/\d{2})?\/\w{8,16}\.\w+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase))
            throw new InvalidOperationException($"The url {url} must be a valid Kook asset URL");

        return true;
    }
}
