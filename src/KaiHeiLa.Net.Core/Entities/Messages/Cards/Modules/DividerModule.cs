using System.Diagnostics;

namespace KaiHeiLa;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DividerModule : IModule
{
    internal DividerModule()
    {
        
    }
    
    public ModuleType Type => ModuleType.Divider;
    
    private string DebuggerDisplay => $"{Type}";

}