namespace Kook;

/// <summary>
///     指示 <see cref="FileAttachment"/> 如何创建附件。
/// </summary>
public enum CreateAttachmentMode
{
    /// <summary>
    ///     通过本地文件路径创建附件。
    /// </summary>
    FilePath,

    /// <summary>
    ///     通过 <see cref="T:System.IO.Stream"/> 流的实例创建附件。
    /// </summary>
    Stream,

    /// <summary>
    ///     通过指向 KOOK 对象存储服务器上的文件的 <see cref="T:System.Uri"/> 创建附件。
    /// </summary>
    AssetUri
}
