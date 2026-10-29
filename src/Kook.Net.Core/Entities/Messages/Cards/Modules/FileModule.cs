using System.Diagnostics;

namespace Kook;

/// <summary>
///     文件模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record FileModule : IMediaModule
{
    internal FileModule(string source, string? title, int? size = null)
    {
        Source = source;
        Title = title;
        Size = size;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <summary>
    ///     获取文件的 URL。
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     获取文件的标题。
    /// </summary>
    public string? Title { get; }

    /// <summary>
    ///     获取文件的大小（单位：字节）。
    /// </summary>
    public int? Size { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";
}
