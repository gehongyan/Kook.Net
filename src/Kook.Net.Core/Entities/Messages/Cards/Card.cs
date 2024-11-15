using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     表示一个卡片对象，可用于卡片消息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Card : ICard, IEquatable<Card>, IEquatable<ICard>
{
    internal Card(CardTheme theme, CardSize size, Color? color, ImmutableArray<IModule> modules)
    {
        Theme = theme;
        Color = color;
        Size = size;
        Modules = modules;
    }

    /// <inheritdoc />
    public CardType Type => CardType.Card;

    /// <inheritdoc />
    public int ModuleCount => Modules.Length;

    /// <summary>
    ///     获取卡片的主题。
    /// </summary>
    public CardTheme Theme { get; }

    /// <summary>
    ///     获取卡片侧边的颜色。
    /// </summary>
    public Color? Color { get; }

    /// <summary>
    ///     获取卡片的大小。
    /// </summary>
    public CardSize Size { get; }

    /// <summary>
    ///     获取卡片的模块。
    /// </summary>
    public ImmutableArray<IModule> Modules { get; }

    private string DebuggerDisplay => $"{Type} ({Modules.Length} Modules)";

    /// <summary>
    ///     判定两个 <see cref="Card"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="Card"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(Card left, Card right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="Card"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="Card"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(Card left, Card right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Card card && Equals(card);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] Card? card) =>
        GetHashCode() == card?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ (Type, Theme, Color, Size).GetHashCode();
            foreach (IModule module in Modules)
                hash = (hash * 16777619) ^ module.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<ICard>.Equals([NotNullWhen(true)] ICard? card) =>
        Equals(card as Card);

    /// <inheritdoc />
    IReadOnlyCollection<IModule> ICard.Modules => Modules;
}
