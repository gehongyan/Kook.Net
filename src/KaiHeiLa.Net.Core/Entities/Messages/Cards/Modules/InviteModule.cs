using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     邀请模块
/// </summary>
/// <remarks>
///     提供服务器邀请/语音频道邀请
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class InviteModule : IModule
{
    internal InviteModule(string code)
    {
        Code = code;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    public string Code { get; }
    
    private string DebuggerDisplay => $"{Type}: {Code}";
    
    public static bool operator ==(InviteModule left, InviteModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(InviteModule left, InviteModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="InviteModule"/>, <see cref="Equals(InviteModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="InviteModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is InviteModule inviteModule && Equals(inviteModule);

    /// <summary>Determines whether the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>.</summary>
    /// <param name="inviteModule">The <see cref="InviteModule"/> to compare with the current <see cref="InviteModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModule"/> is equal to the current <see cref="InviteModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(InviteModule inviteModule)
        => GetHashCode() == inviteModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Code).GetHashCode();
}