namespace Kook;

/// <summary>
///     表一个要上传到 KOOK 的图像。
/// </summary>
public struct Image : IDisposable
{
    private bool _isDisposed;

    /// <summary>
    ///     获取此图像的流。
    /// </summary>
    public Stream Stream { get; }

    internal string? FileExtension { get; }

    /// <summary>
    ///     使用指定的流初始化一个 <see cref="Image"/> 解构的新实例。
    /// </summary>
    /// <param name="stream"> 图像的流。 </param>
    public Image(Stream stream)
    {
        _isDisposed = false;
        Stream = stream;
        if (stream is FileStream fileStream)
            FileExtension = Path.GetExtension(fileStream.Name).Replace(".", "");
    }

    internal Image(Stream stream, string fileExtension)
    {
        _isDisposed = false;
        Stream = stream;
        FileExtension = fileExtension;
    }

    /// <summary>
    ///     通过文件路径创建图像。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <remarks>
    ///     此构造函数会使用 <see cref="System.IO.Path.GetExtension(System.String)"/> 获取文件的扩展名，然后将其直接传递给
    ///     <see cref="System.IO.File.OpenRead(System.String)"/> 方法。
    /// </remarks>
    /// <seealso cref="System.IO.Path.GetExtension(System.String)"/>
    /// <seealso cref="System.IO.File.OpenRead(System.String)"/>
    public Image(string path)
    {
        _isDisposed = false;
        FileExtension = Path.GetExtension(path).Replace(".", "");
        Stream = File.OpenRead(path);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        Stream?.Dispose();
        _isDisposed = true;
    }
}
