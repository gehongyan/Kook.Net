using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     音频模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class AudioModule : IMediaModule, IEquatable<AudioModule>, IEquatable<IModule>
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

    /// <summary>
    ///     判定两个 <see cref="AudioModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AudioModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(AudioModule left, AudioModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="AudioModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AudioModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(AudioModule left, AudioModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is AudioModule audioModule && Equals(audioModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] AudioModule? audioModule) =>
        GetHashCode() == audioModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Source, Title, Cover).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as AudioModule);
}
