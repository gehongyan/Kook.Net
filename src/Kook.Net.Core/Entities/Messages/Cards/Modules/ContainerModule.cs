using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a container module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContainerModule : IModule, IEquatable<ContainerModule>
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
    public ImmutableArray<ImageElement> Elements { get; }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
    
    public static bool operator ==(ContainerModule left, ContainerModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ContainerModule left, ContainerModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ContainerModule"/> is equal to the current <see cref="ContainerModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContainerModule"/>, <see cref="Equals(ContainerModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContainerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModule"/> is equal to the current <see cref="ContainerModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ContainerModule containerModule && Equals(containerModule);

    /// <summary>Determines whether the specified <see cref="ContainerModule"/> is equal to the current <see cref="ContainerModule"/>.</summary>
    /// <param name="containerModule">The <see cref="ContainerModule"/> to compare with the current <see cref="ContainerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModule"/> is equal to the current <see cref="ContainerModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ContainerModule containerModule)
        => GetHashCode() == containerModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            foreach (ImageElement element in Elements)
                hash = (hash * 16777619) ^ element.GetHashCode();
            return hash;
        }
    }
}