namespace KaiHeiLa;

public class ActionGroupModule : IModule
{
    public ActionGroupModule()
    {
        Elements = new List<ButtonElement>();
    }
    
    public ModuleType Type => ModuleType.ActionGroup;
    
    public List<ButtonElement> Elements { get; internal set; }

    public ActionGroupModule Add(ButtonElement element)
    {
        if (Elements.Count >= 4)
        {
            throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 只能有 4 个");
        }
        Elements.Add(element);
        return this;
    }
}