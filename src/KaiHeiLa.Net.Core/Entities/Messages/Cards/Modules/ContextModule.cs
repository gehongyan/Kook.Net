namespace KaiHeiLa;

/// <summary>
///     备注模块
/// </summary>
/// <remarks>
///     展示图文混合的内容
/// </remarks>
public class ContextModule : IModule
{
    public ModuleType Type => ModuleType.Context;
    
    public List<IElement> Elements { get; internal set; }
    
    public ContextModule Add(IElement field)
    {
        if (Elements.Count >= 10)
        {
            throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 最多可包含 10 个元素");
        }
        if (field is not (PlainTextElement or KMarkdownElement or ImageElement))
        {
            throw new ArgumentOutOfRangeException(nameof(field),
                $"{Elements} 可以的元素为 {nameof(PlainTextElement)} 或 {nameof(KMarkdownElement)}");
        }
        Elements.Add(field);
        return this;
    }
}