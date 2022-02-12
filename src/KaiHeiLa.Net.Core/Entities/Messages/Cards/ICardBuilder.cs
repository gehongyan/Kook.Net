namespace KaiHeiLa;

public interface ICardBuilder
{
    CardType Type { get; }
    
    ICard Build();
}