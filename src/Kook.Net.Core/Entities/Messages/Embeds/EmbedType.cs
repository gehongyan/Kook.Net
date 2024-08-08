namespace Kook;

/// <summary>
///     表示嵌入式内容的类型。
/// </summary>
public enum EmbedType
{
    /// <summary>
    ///     嵌入式内容的类型未解析到已知的强类型。
    /// </summary>
    NotImplemented,

    /// <summary>
    ///     嵌入式内容是一个网址链接。
    /// </summary>
    Link,

    /// <summary>
    ///     嵌入式内容是一个图片。
    /// </summary>
    Image,

    /// <summary>
    ///     嵌入式内容是一个哔哩哔哩视频。
    /// </summary>
    BilibiliVideo,

    /// <summary>
    ///     嵌入式内容是一个卡片。
    /// </summary>
    Card,

    // TODO: To be investigated
}
