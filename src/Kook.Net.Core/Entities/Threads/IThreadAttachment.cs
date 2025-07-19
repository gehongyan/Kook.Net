namespace Kook;

/// <summary>
///     表示一个帖子内通用的附件。
/// </summary>
public interface IThreadAttachment
{
    /// <summary>
    ///     获取此附件的类型。
    /// </summary>
    ThreadAttachmentType Type { get; }

    /// <summary>
    ///     获取此附件的 URL。
    /// </summary>
    string Url { get; }

    /// <summary>
    ///     获取此附件的文件名。
    /// </summary>
    string Filename { get; }

    /// <summary>
    ///     获取此附件的封面图片链接。
    /// </summary>
    /// <remarks>
    ///     仅当 <see cref="Type"/> 为 <see cref="ThreadAttachmentType.Video"/> 时有效。
    /// </remarks>
    string? Cover { get; }
}
