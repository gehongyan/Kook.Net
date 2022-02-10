using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

namespace KaiHeiLa;

/// <summary>
///     卡片
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Card : ICard
{
    internal Card(CardTheme theme, CardSize size, ImmutableArray<IModule> modules, Color? color = null)
    {
        Theme = theme;
        Color = color;
        Size = size;
        Modules = modules;
    }

    public CardType Type => CardType.Card;

    public CardTheme Theme { get; internal set; }

    public Color? Color { get; internal set; }

    public CardSize Size { get; internal set; }

    public ImmutableArray<IModule> Modules { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Modules.Length} Modules)";
}