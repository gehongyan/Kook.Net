using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     容器模块
/// </summary>
/// <remarks>
///     1 到多张图片的组合，与图片组模块不同，图片并不会裁切为正方形。多张图片会纵向排列。
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContainerModule : IModule
{
    internal ContainerModule(ImmutableArray<ImageElement> elements)
    {
        Elements = elements;
    }
    
    public ModuleType Type => ModuleType.Container;
    
    public ImmutableArray<ImageElement> Elements { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}