using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a video module in card.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VideoModule : IMediaModule
{
    internal VideoModule(string source, string title)
    {
        Source = source;
        Title = title;
    }

    public ModuleType Type => ModuleType.Video;

    public string Source { get; internal set; }
    
    public string Title { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}