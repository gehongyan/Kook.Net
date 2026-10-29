using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     按钮组模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ActionGroupModule : IModule
{
    internal ActionGroupModule(ImmutableArray<ButtonElement> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ActionGroup;

    /// <summary>
    ///     获取模块的元素。
    /// </summary>
    public ImmutableArray<ButtonElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}
