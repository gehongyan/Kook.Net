namespace Kook;

/// <summary>
///     Represents an icon of an <see cref="IPoke"/>.
/// </summary>
public struct PokeIcon
{
    /// <summary>
    ///     Gets the resource uri of the icon.
    /// </summary>
    public string Resource { get; internal set; }
    /// <summary>
    ///     Gets the resource uri of the icon when the <see cref="IPoke"/> is expired.
    /// </summary>
    public string ResourceExpired { get; internal set; }

    internal PokeIcon(string resource, string resourceExpired)
    {
        Resource = resource;
        ResourceExpired = resourceExpired;
    }

    internal static PokeIcon Create(string resource, string resourceExpired) => new(resource, resourceExpired);
}
