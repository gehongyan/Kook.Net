namespace Kook;

/// <summary>
///     表示一个消息中解析出的网址链接嵌入式内容。
/// </summary>
public struct LinkEmbed : IEmbed
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

    /// <summary>
    ///     获取网址链接的 URL。
    /// </summary>
    public string Url { get; internal set; }

    /// <summary>
    ///     获取链接指向的网站的页面标题。
    /// </summary>
    public string Title { get; internal set; }

    /// <summary>
    ///     获取链接指向的网站的页面描述。
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    ///     获取链接指向的网站的名称。
    /// </summary>
    public string SiteName { get; internal set; }

    /// <summary>
    ///     获取卡片左侧边的颜色。
    /// </summary>
    public Color Color { get; internal set; }

    /// <summary>
    ///     获取预览图像的 URL。
    /// </summary>
    public string Image { get; internal set; }
}
