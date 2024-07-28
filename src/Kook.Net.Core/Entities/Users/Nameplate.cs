using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     表示一个用户的铭牌。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Nameplate : IEquatable<Nameplate>
{
    /// <summary>
    ///     获取此铭牌的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取此铭牌的类型。
    /// </summary>
    public int Type { get; }

    /// <summary>
    ///     获取此铭牌的图标的 URL。
    /// </summary>
    public string Icon { get; }

    /// <summary>
    ///     获取此铭牌的提示信息。
    /// </summary>
    public string Tips { get; }

    private Nameplate(string name, int type, string icon, string tips)
    {
        Name = name;
        Type = type;
        Icon = icon;
        Tips = tips;
    }

    internal static Nameplate Create(string name, int type, string icon, string tips)
    {
        return new Nameplate(name, type, icon, tips);
    }

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
