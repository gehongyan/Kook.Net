namespace KaiHeiLa;

/// <summary>
///     视频模块
/// </summary>
/// <remarks>
///     展示视频
/// </remarks>
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
}