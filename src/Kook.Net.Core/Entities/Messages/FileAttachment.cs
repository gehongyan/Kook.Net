namespace Kook;

/// <summary>
///     表示一个文件附件。
/// </summary>
public struct FileAttachment : IDisposable
{
    private bool _isDisposed;

    /// <summary>
    ///     获取创建此文件附件的方式。
    /// </summary>
    public CreateAttachmentMode Mode { get; }

    /// <summary>
    ///     获取此附件的类型。
    /// </summary>
    public AttachmentType Type { get; }

    /// <summary>
    ///     获取此附件的文件名。
    /// </summary>
    public string FileName { get; }

    /// <summary>
    ///     获取包含文件内容的流。
    /// </summary>
    public Stream? Stream { get; }

    /// <summary>
    ///     获取指向文件的 URL。
    /// </summary>
    public Uri? Uri { get; internal set; }

    /// <summary>
    ///     通过流创建附件。
    /// </summary>
    /// <param name="stream"> 创建附件所使用的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 附件的类型。 </param>
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
    ///     通过文件路径创建附件。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 附件的类型。 </param>
    /// <remarks>
    ///     此构造函数不会校验文件路径的格式，<paramref name="path"/> 的值将会直接传递给
    ///     <see cref="System.IO.File.OpenRead(System.String)"/> 方法。
    /// </remarks>
    /// <seealso cref="System.IO.File.OpenRead(System.String)"/>
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
    ///     通过 URL 创建附件。
    /// </summary>
    /// <remarks>
    ///     URL 应该是指向 KOOK 对象存储服务器上的资源的 URL。如果传入的网络地址不是指向 KOOK 对象存储服务器上的资源的 URL，
    ///     虽然此构造函数不会引发异常，但在发送消息时可能会引发异常。此时，网络资源应先通过转存至 KOOK 对象存储服务器上，再使用此构造函数。
    /// </remarks>
    /// <param name="uri"> 文件的 URL。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 附件的类型。 </param>
    /// <seealso cref="UrlValidation.ValidateKookAssetUrl(System.String)"/>
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
