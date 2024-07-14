using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     An invite module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class InviteModule : IModule, IEquatable<InviteModule>, IEquatable<IModule>
{
    internal InviteModule(string? code)
    {
        Code = code;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    /// <summary>
    ///     Gets the invite code.
    /// </summary>
    public string? Code { get; }

    private string DebuggerDisplay => $"{Type}: {Code}";

    /// <summary>
    ///     判定两个 <see cref="InviteModule"/> 是否相等。
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>;
    /// </returns>
    public static bool operator ==(InviteModule left, InviteModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="InviteModule"/> 是否不相等。
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="InviteModule"/> is not equal to the current <see cref="InviteModule"/>;
    /// </returns>
    public static bool operator !=(InviteModule left, InviteModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is InviteModule inviteModule && Equals(inviteModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] InviteModule? inviteModule) =>
        GetHashCode() == inviteModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Code).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as InviteModule);
}
