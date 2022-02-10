using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     备注模块
/// </summary>
/// <remarks>
///     展示图文混合的内容
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContextModule : IModule
{
    internal ContextModule(ImmutableArray<IElement> elements)
    {
        Elements = elements;
    }

    public ModuleType Type => ModuleType.Context;
    
    public ImmutableArray<IElement> Elements { get; internal set; }
    
    // public ContextModule Add(IElement field)
    // {
    //     if (Elements.Length >= 10)
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 最多可包含 10 个元素");
    //     }
    //     if (field is not (PlainTextElement or KMarkdownElement or ImageElement))
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(field),
    //             $"{Elements} 可以的元素为 {nameof(PlainTextElement)} 或 {nameof(KMarkdownElement)}");
    //     }
    //     Elements.Add(field);
    //     return this;
    // }
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}