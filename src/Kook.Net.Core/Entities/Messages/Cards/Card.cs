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
    ///     Determines whether the specified <see cref="Card"/> is equal to the current <see cref="Card"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="Card"/> is equal to the current <see cref="Card"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(Card left, Card right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="Card"/> is not equal to the current <see cref="Card"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="Card"/> is not equal to the current <see cref="Card"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(Card left, Card right)
        => !(left == right);

    /// <summary>Determines whether the specified object is equal to the current <see cref="Card"/>.</summary>
    /// <remarks>If the object passes is an <see cref="Card"/>, <see cref="Equals(Card)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="Card"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Card"/> is equal to the current <see cref="Card"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Card card && Equals(card);

    /// <summary>Determines whether the specified <see cref="Card"/> is equal to the current <see cref="Card"/>.</summary>
    /// <param name="card">The <see cref="Card"/> to compare with the current <see cref="Card"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Card"/> is equal to the current <see cref="Card"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] Card? card)
        => GetHashCode() == card?.GetHashCode();

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
