using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     图片组模块
/// </summary>
/// <remarks>
///     1 到多张图片的组合
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ImageGroupModule : IModule
{
    internal ImageGroupModule(ImmutableArray<ImageElement> images)
    {
        Elements = images;
    }
    
    public ModuleType Type => ModuleType.ImageGroup;

    public ImmutableArray<ImageElement> Elements { get; internal set; }

    // public ImageGroupModule Add(ImageElement element)
    // {
    //     if (Elements.Count >= 9)
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 只能有 1-9 张图片");
    //     }
    //     Elements.Add(element);
    //     return this;
    // }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}