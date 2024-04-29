using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a divider module builder for creating a <see cref="DividerModule"/>.
/// </summary>
public class DividerModuleBuilder : IModuleBuilder, IEquatable<DividerModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;

    /// <summary>
    ///     Builds this builder into a <see cref="DividerModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="DividerModule"/> representing the built divider module object.
    /// </returns>
    public DividerModule Build() => new();

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(DividerModuleBuilder? left, DividerModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="DividerModuleBuilder"/> is not equal to the current <see cref="DividerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="DividerModuleBuilder"/> is not equal to the current <see cref="DividerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(DividerModuleBuilder? left, DividerModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="DividerModuleBuilder"/>, <see cref="Equals(DividerModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="DividerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is DividerModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>.</summary>
    /// <param name="dividerModuleBuilder">The <see cref="DividerModuleBuilder"/> to compare with the current <see cref="DividerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="DividerModuleBuilder"/> is equal to the current <see cref="DividerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] DividerModuleBuilder? dividerModuleBuilder)
    {
        if (dividerModuleBuilder is null)
            return false;

        return Type == dividerModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as DividerModuleBuilder);
}
