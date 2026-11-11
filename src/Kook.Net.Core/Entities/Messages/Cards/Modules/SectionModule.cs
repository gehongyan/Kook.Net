using System.Diagnostics;

namespace Kook;

/// <summary>
///     内容模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record SectionModule : IModule
{
    internal SectionModule(SectionAccessoryMode? mode, IElement? text = null, IElement? accessory = null)
    {
        Mode = mode;
        Text = text;
        Accessory = accessory;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     获取模块的附加内容的位置。
    /// </summary>
    public SectionAccessoryMode? Mode { get; }

    /// <summary>
    ///     获取模块的文本内容。
    /// </summary>
    public IElement? Text { get; }

    /// <summary>
    ///     获取模块的附加内容。
    /// </summary>
    public IElement? Accessory { get; }

    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";
}
