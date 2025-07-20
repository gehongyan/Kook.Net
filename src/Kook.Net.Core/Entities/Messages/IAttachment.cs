namespace Kook;

/// <summary>
///     表示一个消息内通用的附件。
/// </summary>
public interface IAttachment
{
    /// <summary>
    ///     获取此附件的类型。
    /// </summary>
    AttachmentType Type { get; }

    /// <summary>
    ///     获取此附件的 URL。
    /// </summary>
    string Url { get; }

    /// <summary>
    ///     获取此附件的文件名。
    /// </summary>
    string? Filename { get; }

    /// <summary>
    ///     获取此附件的封面图像的 URL。
    /// </summary>
    string? Cover { get; }

    /// <summary>
    ///     获取此附件的文件大小。
    /// </summary>
    int? Size { get; }

    /// <summary>
    ///     获取此附件的文件类型。
    /// </summary>
    string? FileType { get; }

    /// <summary>
    ///     如果此附件表示可播放的内容，则获取其持续时间。
    /// </summary>
    TimeSpan? Duration { get; }

    /// <summary>
    ///     如果此附件表示的内容包含画面，则获取其宽度。
    /// </summary>
    int? Width { get; }

    /// <summary>
    ///     如果此附件表示的内容包含画面，则获取其高度。
    /// </summary>
    int? Height { get; }
}
