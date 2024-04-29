using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Representing a nameplate an <see cref="IUser"/> can have.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Nameplate : IEquatable<Nameplate>
{
    /// <summary>
    ///     Gets the name of the nameplate given to user.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the type of the nameplate given to user.
    /// </summary>
    public int Type { get; }

    /// <summary>
    ///     Gets the resource uri of the icon of the nameplate given to user.
    /// </summary>
    public string Icon { get; }

    /// <summary>
    ///     Gets the tips of the nameplate given to user.
    /// </summary>
    public string Tips { get; }

    private Nameplate(string name, int type, string icon, string tips)
    {
        Name = name;
        Type = type;
        Icon = icon;
        Tips = tips;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Nameplate"/> class.
    /// </summary>
    /// <param name="name"> The name of the nameplate given to user. </param>
    /// <param name="type"> The type of the nameplate given to user. </param>
    /// <param name="icon"> The resource uri of the icon of the nameplate given to user. </param>
    /// <param name="tips"> The tips of the nameplate given to user. </param>
    /// <returns> A <see cref="Nameplate"/> representing the given parameters. </returns>
    public static Nameplate Create(string name, int type, string icon, string tips) => new(name, type, icon, tips);

    private string DebuggerDisplay => Name;

    #region IEquatable

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] Nameplate? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Name == other.Name
            && Type == other.Type
            && Icon == other.Icon
            && Tips == other.Tips;
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;

        return Equals((Nameplate)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ Name.GetHashCode();
            hash = (hash * 16777619) ^ Type.GetHashCode();
            hash = (hash * 16777619) ^ Icon.GetHashCode();
            hash = (hash * 16777619) ^ Tips.GetHashCode();
            return hash;
        }
    }

    #endregion
}
