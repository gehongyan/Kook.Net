using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a context module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContextModule : IModule
{
    internal ContextModule(ImmutableArray<IElement> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Context;
    
    /// <summary>
    ///     Gets the elements in this context module.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{IElement}"/> representing the elements in this context module.
    /// </returns>
    public ImmutableArray<IElement> Elements { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}