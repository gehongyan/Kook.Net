using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A file module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FileModule : IMediaModule
{
    internal FileModule(string source, string title)
    {
        Source = source;
        Title = title;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.File;

    /// <inheritdoc />
    public string Source { get; internal set; }
    
    /// <inheritdoc />
    public string Title { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Title}";
}