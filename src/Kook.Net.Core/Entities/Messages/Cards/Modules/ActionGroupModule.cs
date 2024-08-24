using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     按钮组模块，可用于 <see cref="ICard"/> 中。
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
    ///     获取模块的元素。
    /// </summary>
    public ImmutableArray<ButtonElement> Elements { get; }

    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";

    /// <summary>
    ///     判定两个 <see cref="ActionGroupModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ActionGroupModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ActionGroupModule left, ActionGroupModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ActionGroupModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ActionGroupModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ActionGroupModule left, ActionGroupModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ActionGroupModule actionGroupModule && Equals(actionGroupModule);

    /// <inheritdoc />
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
