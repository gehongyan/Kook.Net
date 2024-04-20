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
    ///     Determines whether the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>;
    /// </returns>
    public static bool operator ==(InviteModule left, InviteModule right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="InviteModule"/> is not equal to the current <see cref="InviteModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="InviteModule"/> is not equal to the current <see cref="InviteModule"/>;
    /// </returns>
    public static bool operator !=(InviteModule left, InviteModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="InviteModule"/>, <see cref="Equals(InviteModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="InviteModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is InviteModule inviteModule && Equals(inviteModule);

    /// <summary>Determines whether the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>.</summary>
    /// <param name="inviteModule">The <see cref="InviteModule"/> to compare with the current <see cref="InviteModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] InviteModule? inviteModule)
        => GetHashCode() == inviteModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Code).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as InviteModule);
}
