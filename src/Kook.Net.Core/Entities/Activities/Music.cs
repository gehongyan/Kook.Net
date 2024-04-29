namespace Kook;

/// <summary>
///     Represents a music activity.
/// </summary>
public class Music
{
    /// <summary>
    ///     Gets or sets the music provider.
    /// </summary>
    public MusicProvider Provider { get; set; }

    /// <summary>
    ///     Gets or sets the music ID.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the singer.
    /// </summary>
    public string? Singer { get; set; }
}
