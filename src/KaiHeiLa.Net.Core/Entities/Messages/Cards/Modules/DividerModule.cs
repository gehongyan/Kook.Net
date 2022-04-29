using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A divider module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DividerModule : IModule
{
    internal DividerModule()
    {
        
    }
    
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Divider;
    
    private string DebuggerDisplay => $"{Type}";

}