using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     容器模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ContainerModule : IModule
{
    internal ContainerModule(ImmutableArray<ImageElement> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     获取模块的元素。
    /// </summary>
    public ImmutableArray<ImageElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}
