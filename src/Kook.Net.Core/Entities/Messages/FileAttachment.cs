using Kook.Utils;

namespace Kook;

/// <summary>
///     Represents a file attachment.
/// </summary>
public struct FileAttachment : IDisposable
{
    private bool _isDisposed;

    /// <summary>
    ///     Gets how this attachment will be operated.
    /// </summary>
    public CreateAttachmentMode Mode { get; private set; }

    /// <summary>
    ///     Gets the type of this attachment.
    /// </summary>
    public AttachmentType Type { get; private set; }

    /// <summary>
    ///     Gets the filename.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    ///     Gets the stream containing the file content.
    /// </summary>
    public Stream? Stream { get; }

    /// <summary>
    ///     Gets the URI of the file.
    /// </summary>
    public Uri? Uri { get; internal set; }

    /// <summary>
    ///     Creates a file attachment from a stream.
    /// </summary>
    /// <param name="stream">The stream to create the attachment from.</param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type">The type of the attachment.</param>
    public FileAttachment(Stream stream, string filename, AttachmentType type = AttachmentType.File)
    {
        _isDisposed = false;
        Mode = CreateAttachmentMode.Stream;
        Type = type;
        FileName = filename;
        Stream = stream;
        try
        {
            Stream.Position = 0;
        }
        catch
        {
            // ignored
        }

        Uri = null;
    }

    /// <summary>
    ///     Create the file attachment from a file path.
    /// </summary>
    /// <remarks>
    ///     This file path is NOT validated and is passed directly into a
    ///     <see cref="File.OpenRead"/>.
    /// </remarks>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type">The type of the attachment.</param>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or
    ///     more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="path" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
    ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260
    ///     characters.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    ///     <paramref name="path" /> is in an invalid format.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     The specified <paramref name="path"/> is invalid, (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">
    ///     <paramref name="path" /> specified a directory. -or- The caller does not have the required permission.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     The file specified in <paramref name="path" /> was not found.
    /// </exception>
    /// <exception cref="IOException">
    ///     An I/O error occurred while opening the file.
    /// </exception>
    public FileAttachment(string path, string? filename = null, AttachmentType type = AttachmentType.File)
    {
        _isDisposed = false;
        Mode = CreateAttachmentMode.FilePath;
        Type = type;
        Stream = File.OpenRead(path);
        FileName = filename ?? Path.GetFileName(path);
        Uri = null;
    }

    /// <summary>
    ///     Create the file attachment from a URI.
    /// </summary>
    /// <remarks>
    ///     This URI path will be validated before being passed into REST API.
    ///     If the resource the URI points to is not stored on KOOK OSS, this constructor will throw an exception.
    ///     Under this circumstance, please create asset in advance.
    /// </remarks>
    /// <param name="uri">The URI of the file.</param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type">The type of the attachment.</param>
    /// <exception cref="InvalidOperationException">The URI provided is not an asset on the KOOK OSS.</exception>
    /// <exception cref="ArgumentException">The URI provided is blank.</exception>
    /// <seealso cref="UrlValidation.ValidateKookAssetUrl"/>
    public FileAttachment(Uri uri, string filename, AttachmentType type = AttachmentType.File)
    {
        _isDisposed = false;
        Mode = CreateAttachmentMode.AssetUri;
        Type = type;
        Stream = null;
        FileName = filename;
        Uri = uri;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isDisposed)
        {
            Stream?.Dispose();
            _isDisposed = true;
        }
    }
}
