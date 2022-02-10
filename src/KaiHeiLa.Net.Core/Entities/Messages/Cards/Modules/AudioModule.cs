using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     音频模块
/// </summary>
/// <remarks>
///     展示音频
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class AudioModule : IMediaModule
{
    internal AudioModule(string source, string title, string cover)
    {
        Source = source;
        Title = title;
        Cover = cover;
    }

    public ModuleType Type => ModuleType.Audio;

    public string Source { get; internal set; }
    
    public string Title { get; internal set; }
    
    public string Cover { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}