using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a container module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContainerModule : IModule
{
    internal ContainerModule(ImmutableArray<ImageElement> elements)
    {
        Elements = elements;
    }
    
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;
    
    /// <summary>
    ///     Gets the elements in this container module.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{ImageElement}"/> representing the elements in this container module.
    /// </returns>
    public ImmutableArray<ImageElement> Elements { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}