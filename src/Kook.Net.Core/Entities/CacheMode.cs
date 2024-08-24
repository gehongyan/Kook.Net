namespace Kook;

/// <summary>
///     表示应该使用的缓存模式。
/// </summary>
public enum CacheMode
{
    /// <summary>
    ///     允许在实体不存在于现有缓存中时下载对象。
    /// </summary>
    AllowDownload,

    /// <summary>
    ///     仅允许从现有缓存中提取对象。
    /// </summary>
    CacheOnly
}
