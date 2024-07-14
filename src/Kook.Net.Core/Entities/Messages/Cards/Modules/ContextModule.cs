using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a context module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ContextModule : IModule, IEquatable<ContextModule>, IEquatable<IModule>
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

    /// <summary>
    ///     判定两个 <see cref="ContextModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ContextModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ContextModule left, ContextModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ContextModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ContextModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ContextModule left, ContextModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ContextModule contextModule && Equals(contextModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ContextModule? contextModule) =>
        GetHashCode() == contextModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            foreach (IElement element in Elements)
                hash = (hash * 16777619) ^ element.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as ContextModule);
}
