using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents an audio module that can be used in an <see cref="ICard"/>.
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

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public string? Title { get; }

    /// <summary>
    ///     Gets the cover of the audio associated with this module.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the cover of the audio associated with this module.
    /// </returns>
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
