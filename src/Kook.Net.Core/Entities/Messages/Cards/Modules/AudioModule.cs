using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents an audio module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class AudioModule : IMediaModule, IEquatable<AudioModule>
{
    internal AudioModule(string source, string title, string cover)
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
    public string Title { get; }

    /// <summary>
    ///     Gets the cover of the audio associated with this module.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the cover of the audio associated with this module.
    /// </returns>
    public string Cover { get; }

    private string DebuggerDisplay => $"{Type}: {Title}";

    public static bool operator ==(AudioModule left, AudioModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(AudioModule left, AudioModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="AudioModule"/> is equal to the current <see cref="AudioModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="AudioModule"/>, <see cref="Equals(AudioModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="AudioModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="AudioModule"/> is equal to the current <see cref="AudioModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is AudioModule audioModule && Equals(audioModule);

    /// <summary>Determines whether the specified <see cref="AudioModule"/> is equal to the current <see cref="AudioModule"/>.</summary>
    /// <param name="audioModule">The <see cref="AudioModule"/> to compare with the current <see cref="AudioModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="AudioModule"/> is equal to the current <see cref="AudioModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(AudioModule audioModule)
        => GetHashCode() == audioModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Source, Title, Cover).GetHashCode();
}
