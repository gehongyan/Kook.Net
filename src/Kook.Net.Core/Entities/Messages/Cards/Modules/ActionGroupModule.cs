using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents an action group module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ActionGroupModule : IModule, IEquatable<ActionGroupModule>, IEquatable<IModule>
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
    public ImmutableArray<ButtonElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ActionGroupModule left, ActionGroupModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModule"/> is not equal to the current <see cref="ActionGroupModule"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModule"/> is not equal to the current <see cref="ActionGroupModule"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ActionGroupModule left, ActionGroupModule right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ActionGroupModule"/>, <see cref="Equals(ActionGroupModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ActionGroupModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ActionGroupModule actionGroupModule && Equals(actionGroupModule);

    /// <summary>Determines whether the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>.</summary>
    /// <param name="actionGroupModule">The <see cref="ActionGroupModule"/> to compare with the current <see cref="ActionGroupModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModule"/> is equal to the current <see cref="ActionGroupModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ActionGroupModule? actionGroupModule) =>
        GetHashCode() == actionGroupModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ Type.GetHashCode();
            foreach (ButtonElement buttonElement in Elements)
                hash = (hash * 16777619) ^ buttonElement.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as ActionGroupModule);
}
