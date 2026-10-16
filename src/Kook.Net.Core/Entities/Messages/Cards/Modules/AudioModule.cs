using System.Diagnostics;

namespace Kook;

/// <summary>
///     音频模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record AudioModule : IMediaModule
{
    internal AudioModule(string source, string? title, string? cover)
    {
        Source = source;
        Title = title;
        Cover = cover;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Audio;

    /// <summary>
    ///     获取音频的 URL。
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     获取音频的标题。
    /// </summary>
    public string? Title { get; }

    /// <summary>
    ///     获取音频的封面的 URL。
    /// </summary>
    public string? Cover { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";
}
