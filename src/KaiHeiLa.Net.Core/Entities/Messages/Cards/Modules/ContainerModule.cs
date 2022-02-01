namespace KaiHeiLa;

/// <summary>
///     容器模块
/// </summary>
/// <remarks>
///     1 到多张图片的组合，与图片组模块不同，图片并不会裁切为正方形。多张图片会纵向排列。
/// </remarks>
public class ContainerModule : IModule
{
    public ContainerModule()
    {
        Elements = new List<ImageElement>();
    }
    
    public ModuleType Type => ModuleType.Container;
    
    public List<ImageElement> Elements { get; internal set; }

    public ContainerModule Add(ImageElement element)
    {
        if (Elements.Count >= 9)
        {
            throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 只能有 1-9 张图片");
        }
        Elements.Add(element);
        return this;
    }
}