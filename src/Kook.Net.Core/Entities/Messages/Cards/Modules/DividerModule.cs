using System.Diagnostics;

namespace Kook;

/// <summary>
///     分割线模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record DividerModule : IModule
{
    internal DividerModule()
    {
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;

    private string DebuggerDisplay => $"{Type}";
}
