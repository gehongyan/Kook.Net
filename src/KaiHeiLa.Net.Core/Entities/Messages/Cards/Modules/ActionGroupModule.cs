using System.Collections.Immutable;
using System.Diagnostics;

namespace KaiHeiLa;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ActionGroupModule : IModule
{
    internal ActionGroupModule(ImmutableArray<ButtonElement> elements)
    {
        Elements = elements;
    }
    
    public ModuleType Type => ModuleType.ActionGroup;
    
    public ImmutableArray<ButtonElement> Elements { get; internal set; }

    // internal ActionGroupModule Add(ButtonElement element)
    // {
    //     if (Elements.Length >= 4)
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(Elements), $"{nameof(Elements)} 只能有 4 个");
    //     }
    //     Elements = Elements.Add(element);
    //     return this;
    // }
    
    private string DebuggerDisplay => $"{Type} ({Elements.Length} Elements)";
}