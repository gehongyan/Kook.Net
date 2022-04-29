using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a section module in card.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SectionModule : IModule
{
    internal SectionModule(SectionAccessoryMode mode, IElement text, IElement accessory = null)
    {
        Mode = mode;
        Text = text;
        Accessory = accessory;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     Specifies that the <see cref="Accessory"/> is to the left or right of <see cref="Text"/>.
    /// </summary>
    /// <returns>
    ///     <see cref="SectionAccessoryMode.Left"/> if the <see cref="Accessory"/> is to the left of <see cref="Text"/>,
    ///     <see cref="SectionAccessoryMode.Right"/> if the <see cref="Accessory"/> is to the right of <see cref="Text"/>,
    ///     <see cref="SectionAccessoryMode.Unspecified"/> if how the <see cref="Accessory"/> is positioned is not specified.
    /// </returns>
    public SectionAccessoryMode Mode { get; internal set; }

    /// <summary>
    ///     Gets the text of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> representing the text of the section.
    /// </returns>
    public IElement Text { get; internal set; }

    /// <summary>
    ///     Gets the accessory of the section.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> representing the accessory of the section.
    /// </returns>
    public IElement Accessory { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";
}