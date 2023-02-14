namespace Kook;

/// <summary>
///     Represents a message attachment found in a <see cref="IUserMessage"/>.
/// </summary>
public interface IAttachment
{
    /// <summary>
    ///     Gets the type of the attachment.
    /// </summary>
    /// <returns>
    ///     An <see cref="AttachmentType"/> representing the type of the attachment.
    /// </returns>
    AttachmentType Type { get; }

    /// <summary>
    ///     Gets the URL of the attachment.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the URL of the attachment.
    /// </returns>
    string Url { get; }

    /// <summary>
    ///     Gets the filename of this attachment.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> containing the full filename of this attachment.
    /// </returns>
    string Filename { get; }

    /// <summary>
    ///     Gets the file size of the attachment.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> representing the file size of the attachment;
    ///     <c>null</c> if the file size is unknown or not applicable.
    /// </returns>
    int? Size { get; }

    /// <summary>
    ///     Gets the file type of the attachment.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the file type of the attachment;
    ///     <c>null</c> if the file type is unknown or not applicable.
    /// </returns>
    string FileType { get; }

    /// <summary>
    ///     Gets the duration of the attachment.
    /// </summary>
    /// <remarks>
    ///     A timespan representing the duration of the attachment;
    ///     <c>null</c> if the duration is unknown or not applicable.
    /// </remarks>
    TimeSpan? Duration { get; }

    /// <summary>
    ///     Gets the width of the attachment.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> representing the width of the attachment;
    ///     <c>null</c> if the width is unknown or not applicable.
    /// </returns>
    int? Width { get; }

    /// <summary>
    ///     Gets the height of the attachment.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> representing the height of the attachment;
    ///     <c>null</c> if the height is unknown or not applicable.
    /// </returns>
    int? Height { get; }
}
