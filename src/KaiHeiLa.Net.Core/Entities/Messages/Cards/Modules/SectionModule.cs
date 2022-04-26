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

    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     Specifies that the <see cref="Accessory"/> is on the left or right of <see cref="Text"/>
    /// </summary>
    public SectionAccessoryMode Mode { get; internal set; }

    public IElement Text { get; internal set; }

    public IElement Accessory { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";
}