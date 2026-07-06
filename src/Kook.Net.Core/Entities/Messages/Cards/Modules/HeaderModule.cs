using System.Diagnostics;

namespace Kook;

/// <summary>
///     标题模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record HeaderModule : IModule
{
    internal HeaderModule(PlainTextElement? text)
    {
        Text = text;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     获取模块的标题内容。
    /// </summary>
    public PlainTextElement? Text { get; }

    /// <inheritdoc />
    public override string? ToString() => Text?.ToString();

    private string DebuggerDisplay => $"{Type}: {Text}";
}
