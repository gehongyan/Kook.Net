using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     内容模块
/// </summary>
/// <remarks>
///     结构化的内容，显示文本+其它元素
/// </remarks>
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
    ///     mode 代表 accessory 是放置在左侧还是在右侧
    /// </summary>
    public SectionAccessoryMode Mode { get; internal set; }

    public IElement Text { get; internal set; }

    public IElement Accessory { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";
}