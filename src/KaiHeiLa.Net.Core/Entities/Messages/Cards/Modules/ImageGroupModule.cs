namespace KaiHeiLa;

/// <summary>
///     图片组模块
/// </summary>
/// <remarks>
///     1 到多张图片的组合
/// </remarks>
public class ImageGroupModule : IModule
{
    public ImageGroupModule()
    {
        Elements = new List<ImageElement>();
    }
    
    public ModuleType Type => ModuleType.ImageGroup;

    public List<ImageElement> Elements { get; internal set; }

    public ImageGroupModule Add(ImageElement element)
    {
        if (Elements.Count >= 9)
        {
            throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 只能有 1-9 张图片");
        }
        Elements.Add(element);
        return this;
    }
}