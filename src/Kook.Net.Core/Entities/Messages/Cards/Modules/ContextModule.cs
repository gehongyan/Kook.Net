using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

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
    public ImmutableArray<IElement> Elements { get; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
    
    public static bool operator ==(ContextModule left, ContextModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ContextModule left, ContextModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ContextModule"/> is equal to the current <see cref="ContextModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContextModule"/>, <see cref="Equals(ContextModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContextModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModule"/> is equal to the current <see cref="ContextModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ContextModule contextModule && Equals(contextModule);

    /// <summary>Determines whether the specified <see cref="ContextModule"/> is equal to the current <see cref="ContextModule"/>.</summary>
    /// <param name="contextModule">The <see cref="ContextModule"/> to compare with the current <see cref="ContextModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModule"/> is equal to the current <see cref="ContextModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ContextModule contextModule)
        => GetHashCode() == contextModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            foreach (IElement element in Elements)
                hash = (hash * 16777619) ^ element.GetHashCode();
            return hash;
        }
    }
}