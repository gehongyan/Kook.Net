using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个卡片对象，可用于卡片消息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record Card : ICard
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

    /// <inheritdoc />
    IReadOnlyCollection<IModule> ICard.Modules => Modules;
}
