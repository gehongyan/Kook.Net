using System.Diagnostics;

namespace Kook;

/// <summary>3
///     邀请模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record InviteModule : IModule
{
    internal InviteModule(string? code)
    {
        Code = code;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    /// <summary>
    ///     获取邀请代码。
    /// </summary>
    public string? Code { get; }

    private string DebuggerDisplay => $"{Type}: {Code}";
}
