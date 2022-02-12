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
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}