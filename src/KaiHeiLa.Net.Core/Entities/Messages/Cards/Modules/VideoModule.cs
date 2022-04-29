using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a video module in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VideoModule : IMediaModule
{
    internal VideoModule(string source, string title)
    {
        Source = source;
        Title = title;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Video;

    /// <inheritdoc />
    public string Source { get; internal set; }
    
    /// <inheritdoc />
    public string Title { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}