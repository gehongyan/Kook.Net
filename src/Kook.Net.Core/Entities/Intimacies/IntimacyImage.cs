namespace Kook;

/// <summary>
///     表示一个亲密度的形象图像。
/// </summary>
public class IntimacyImage
{
    internal IntimacyImage(uint id, string url)
    {
        Id = id;
        Url = url;
    }

    /// <summary>
    ///     获取此亲密度的形象图像的唯一标识符。
    /// </summary>
    public uint Id { get; }

    /// <summary>
    ///     获取此亲密度的形象图像的 URL。
    /// </summary>
    public string Url { get; }
}
