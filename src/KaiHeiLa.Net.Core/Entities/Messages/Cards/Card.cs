using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

namespace KaiHeiLa;

/// <summary>
///     Represents a card object seen in an <see cref="IUserMessage"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Card : ICard
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
    public CardTheme Theme { get; internal set; }

    /// <summary>
    ///     Gets the color of this embed.
    /// </summary>
    /// <returns>
    ///     A <see cref="Color"/> represents a color present on the side of the card, or <c>null</c> if none is set.
    /// </returns>
    public Color? Color { get; internal set; }

    /// <summary>
    ///     Gets the size of this card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardSize"/> value that represents the size of this card.
    /// </returns>
    public CardSize Size { get; internal set; }

    /// <summary>
    ///     Gets the modules in this card.
    /// </summary>
    /// <returns>
    ///     An array of the modules of the card.
    /// </returns>
    public ImmutableArray<IModule> Modules { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Modules.Length} Modules)";
}