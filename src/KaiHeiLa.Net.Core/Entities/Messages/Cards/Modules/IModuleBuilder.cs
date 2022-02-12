namespace KaiHeiLa;

public interface IModuleBuilder
{
    ModuleType Type { get; }
    
    IModule Build();
}