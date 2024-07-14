using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a video module in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VideoModule : IMediaModule, IEquatable<VideoModule>, IEquatable<IModule>
{
    internal VideoModule(string source, string? title)
    {
        Source = source;
        Title = title;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string? Title { get; }

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
