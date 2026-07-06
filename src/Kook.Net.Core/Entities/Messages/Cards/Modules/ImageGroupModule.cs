using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     图片组模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ImageGroupModule : IModule
{
    internal ImageGroupModule(ImmutableArray<ImageElement> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ImageGroup;

    /// <summary>
    ///     获取模块的元素。
    /// </summary>
    public ImmutableArray<ImageElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}
