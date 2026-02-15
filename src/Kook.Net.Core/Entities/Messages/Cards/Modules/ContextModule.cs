using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     备注模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ContextModule : IModule
{
    internal ContextModule(ImmutableArray<IElement> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Context;

    /// <summary>
    ///     获取模块的元素。
    /// </summary>
    public ImmutableArray<IElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}
