using System.Diagnostics;

namespace Kook;

/// <summary>
///     视频模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record VideoModule : IMediaModule
{
    internal VideoModule(string source, string? title, string? cover,
        int? size, TimeSpan? duration, int? width, int? height)
    {
        Source = source;
        Title = title;
        Cover = cover;
        Size = size;
        Duration = duration;
        Width = width;
        Height = height;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <summary>
    ///     获取视频的 URL。
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     获取视频的标题。
    /// </summary>
    public string? Title { get; }

    /// <summary>
    ///     获取视频的封面。
    /// </summary>
    public string? Cover { get; }

    /// <summary>
    ///     获取此视频的文件大小。
    /// </summary>
    public int? Size { get; }

    /// <summary>
    ///     获取此视频的持续时间。
    /// </summary>
    public TimeSpan? Duration { get; }

    /// <summary>
    ///     获取此视频的宽度。
    /// </summary>
    public int? Width { get; }

    /// <summary>
    ///     获取此视频的高度。
    /// </summary>
    public int? Height { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";
}
