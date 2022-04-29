using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents an audio module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class AudioModule : IMediaModule
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
    public string Source { get; internal set; }
    
    /// <inheritdoc />
    public string Title { get; internal set; }
    
    /// <summary>
    ///     Gets the cover of the audio associated with this module.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the cover of the audio associated with this module.
    /// </returns>
    public string Cover { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}