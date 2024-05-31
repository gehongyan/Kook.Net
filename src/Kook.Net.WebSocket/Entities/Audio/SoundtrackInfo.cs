namespace Kook.Audio;

/// <summary>
///     Represents the information of a soundtrack.
/// </summary>
public readonly struct SoundtrackInfo
{
    /// <summary>
    ///     Gets the name of the software from which the soundtrack audio originates
    /// </summary>
    public string? Software { get; init; }

    /// <summary>
    ///     Gets the name of the music soundtrack
    /// </summary>
    public string? Music { get; init; }

    /// <summary>
    ///     Gets the singer of the music soundtrack
    /// </summary>
    public string? Singer { get; init; }
}
