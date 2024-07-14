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
    ///     判定两个 <see cref="DividerModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="DividerModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(DividerModuleBuilder? left, DividerModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="DividerModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="DividerModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(DividerModuleBuilder? left, DividerModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is DividerModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
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
