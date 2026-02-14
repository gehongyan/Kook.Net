namespace Kook;

/// <summary>
///     表示一个服务器靓号在服务器内的设置信息。
/// </summary>
public readonly struct RareGuildResources
{
    /// <summary>
    ///     获取具有靓号的服务器的图标的边框图片集合，键为边框图片资源的名称，值为边框图片的 URL 地址。
    /// </summary>
    public required IReadOnlyDictionary<string, string> FrameImages { get; init; }

    /// <summary>
    ///     获取具有靓号的服务器图标的边框颜色。
    /// </summary>
    public required Color FrameColor { get; init; }

    /// <summary>
    ///     获取服务器靓号标签的图片集合，键为标签图片资源的名称，值为标签图片的 URL 地址。
    /// </summary>
    public required IReadOnlyDictionary<string, string> NameplateImages { get; init; }

    /// <summary>
    ///     获取服务器的封面的图片 URL 地址。
    /// </summary>
    public required string CoverImage { get; init; }

    /// <summary>
    ///     获取服务器靓号的简介信息。
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    ///     获取服务器靓号的描述信息。
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    ///     获取服务器靓号标签的背景图像。
    /// </summary>
    public required string IdIcon { get; init; }

    /// <summary>
    ///     获取服务器靓号关联的语音频道链接。
    /// </summary>
    public string? VoiceChannelLink { get; init; }
}
