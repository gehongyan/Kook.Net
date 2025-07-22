using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     视频模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VideoModule : IMediaModule, IEquatable<VideoModule>, IEquatable<IModule>
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

    /// <summary>
    ///     判定两个 <see cref="VideoModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="VideoModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(VideoModule left, VideoModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="VideoModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="VideoModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(VideoModule left, VideoModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is VideoModule videoModule && Equals(videoModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)]VideoModule? videoModule) =>
        GetHashCode() == videoModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Source, Title).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as VideoModule);
}
