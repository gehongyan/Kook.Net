using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     文件模块
/// </summary>
/// <remarks>
///     展示文件
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FileModule : IMediaModule
{
    internal FileModule(string source, string title)
    {
        Source = source;
        Title = title;
    }

    public ModuleType Type => ModuleType.File;

    public string Source { get; internal set; }
    
    public string Title { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}