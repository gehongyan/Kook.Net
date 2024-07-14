using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a card object seen in an <see cref="IUserMessage"/>.
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
    ///     Gets the theme of this card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardTheme"/> value that represents the theme of this card.
    /// </returns>
    public CardTheme Theme { get; }

    /// <summary>
    ///     Gets the color of this embed.
    /// </summary>
    /// <returns>
    ///     A <see cref="Color"/> represents a color present on the side of the card, or <c>null</c> if none is set.
    /// </returns>
    public Color? Color { get; }

    /// <summary>
    ///     Gets the size of this card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardSize"/> value that represents the size of this card.
    /// </returns>
    public CardSize Size { get; }

    /// <summary>
    ///     Gets the modules in this card.
    /// </summary>
    /// <returns>
    ///     An array of the modules of the card.
    /// </returns>
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
}
