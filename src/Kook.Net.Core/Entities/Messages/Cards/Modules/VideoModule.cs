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
    ///     Determines whether the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(VideoModule left, VideoModule right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="VideoModule"/> is not equal to the current <see cref="VideoModule"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="VideoModule"/> is not equal to the current <see cref="VideoModule"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(VideoModule left, VideoModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="VideoModule"/>, <see cref="Equals(VideoModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="VideoModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)]object? obj)
        => obj is VideoModule videoModule && Equals(videoModule);

    /// <summary>Determines whether the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>.</summary>
    /// <param name="videoModule">The <see cref="VideoModule"/> to compare with the current <see cref="VideoModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="VideoModule"/> is equal to the current <see cref="VideoModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)]VideoModule? videoModule)
        => GetHashCode() == videoModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Source, Title).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as VideoModule);
}
