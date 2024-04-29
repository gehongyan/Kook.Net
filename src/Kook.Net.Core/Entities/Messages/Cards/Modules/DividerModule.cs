using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     A divider module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DividerModule : IModule, IEquatable<DividerModule>, IEquatable<IModule>
{
    internal DividerModule()
    {
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;

    private string DebuggerDisplay => $"{Type}";

    /// <summary>
    ///     Determines whether the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(DividerModule left, DividerModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="DividerModule"/> is not equal to the current <see cref="DividerModule"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="DividerModule"/> is not equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(DividerModule left, DividerModule right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="DividerModule"/>, <see cref="Equals(DividerModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="DividerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is DividerModule dividerModule && Equals(dividerModule);

    /// <summary>Determines whether the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>.</summary>
    /// <param name="dividerModule">The <see cref="DividerModule"/> to compare with the current <see cref="DividerModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModule"/> is equal to the current <see cref="DividerModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] DividerModule? dividerModule) =>
        GetHashCode() == dividerModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        Type.GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as DividerModule);
}
