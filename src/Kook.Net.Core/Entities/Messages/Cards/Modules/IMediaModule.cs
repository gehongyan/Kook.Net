namespace Kook;

/// <summary>
///     A generic media module that can be used in an <see cref="ICard"/>.
/// </summary>
public interface IMediaModule : IModule
{
    /// <summary>
    ///     Gets the source of the media associated with this module.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the source of the audio associated with this module.
    /// </returns>
    string Source { get; }

    /// <summary>
    ///     Gets the title of the media associated with this module.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the title of the media associated with this module.
    /// </returns>
    string? Title { get; }
}
