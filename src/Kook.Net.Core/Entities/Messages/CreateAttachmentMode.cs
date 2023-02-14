namespace Kook;

/// <summary>
///     Indicates that how the <see cref="FileAttachment"/> will be operated to attache files.
/// </summary>
public enum CreateAttachmentMode
{
    /// <summary>
    ///     The <see cref="FileAttachment"/> will be created via a local file path.
    /// </summary>
    FilePath,

    /// <summary>
    ///     The <see cref="FileAttachment"/> will be created via a <see cref="Stream"/>.
    /// </summary>
    Stream,

    /// <summary>
    ///     The <see cref="FileAttachment"/> will be created via a <see cref="System.Uri"/>
    ///     pointing to a file on KOOK asset OSS.
    /// </summary>
    AssetUri
}
