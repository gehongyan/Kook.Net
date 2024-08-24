namespace Kook;

/// <summary>
///     表示一个 POKE 的图标。
/// </summary>
public struct PokeIcon
{
    /// <summary>
    ///     获取图标资源的 URL。
    /// </summary>
    public string Resource { get; }

    /// <summary>
    ///     获取当此图标所关联的 POKE 过期后的图标资源的 URL。
    /// </summary>
    public string ResourceExpired { get; }

    internal PokeIcon(string resource, string resourceExpired)
    {
        Resource = resource;
        ResourceExpired = resourceExpired;
    }

    internal static PokeIcon Create(string resource, string resourceExpired) => new(resource, resourceExpired);
}
