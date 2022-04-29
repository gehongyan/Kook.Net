using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents an action group module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ActionGroupModule : IModule
{
    internal ActionGroupModule(ImmutableArray<ButtonElement> elements)
    {
        Elements = elements;
    }
    
    /// <inheritdoc />
    public ModuleType Type => ModuleType.ActionGroup;
    
    /// <summary>
    ///     Gets the elements of this module.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{ButtonElement}"/> containing the elements of this module.
    /// </returns>
    public ImmutableArray<ButtonElement> Elements { get; internal set; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}