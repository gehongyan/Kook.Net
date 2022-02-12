namespace KaiHeiLa;

public interface ICard
{
    CardType Type { get; }
    
    int ModuleCount { get; }
}