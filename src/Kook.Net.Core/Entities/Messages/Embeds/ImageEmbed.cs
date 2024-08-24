namespace Kook;

/// <summary>
///     表示一个消息中解析出的图片嵌入式内容。
/// </summary>
public struct ImageEmbed : IEmbed
{
    internal ImageEmbed(string url, string originUrl)
    {
        Url = url;
        OriginUrl = originUrl;
    }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.Image;

    /// <summary>
    ///     获取图像的 URL。
    /// </summary>
    public string Url { get; }

    /// <summary>
    ///     获取嵌入式内容所解析的原始 URL。
    /// </summary>
    public string OriginUrl { get; }
}
