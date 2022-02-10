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
    // {
    //     get => _mode;
    //     internal set
    //     {
    //         if (value == SectionAccessoryMode.Left && Accessory is ButtonElement)
    //         {
    //             throw new InvalidOperationException("button 不能放置在左侧");
    //         }
    //         _mode = value;
    //     }
    // }

    public IElement Text { get; internal set; }
    // {
    //     get => _text;
    //     internal set
    //     {
    //         if (value is not (PlainTextElement or KMarkdownElement))
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(Text),
    //                 $"{Text} 可以的元素为 {nameof(PlainTextElement)}, {nameof(KMarkdownElement)} 或 {nameof(ParagraphStruct)}");
    //         }
    //         _text = value;
    //     }
    // }

    public IElement Accessory { get; internal set; }
    // {
    //     get => _accessory;
    //     internal set
    //     {
    //         if (value is not (ImageElement or ButtonElement))
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(Accessory),
    //                 $"{Accessory} 可以的元素为 {nameof(ImageElement)} 或 {nameof(ButtonElement)}");
    //         }
    //         if (Mode == SectionAccessoryMode.Left && value is ButtonElement)
    //         {
    //             throw new InvalidOperationException("button 不能放置在左侧");
    //         }
    //         _accessory = value;
    //     }
    // }
    
    private string DebuggerDisplay => $"{Type}: {Text}{(Accessory is null ? string.Empty : $"{Mode} Accessory")}";
}