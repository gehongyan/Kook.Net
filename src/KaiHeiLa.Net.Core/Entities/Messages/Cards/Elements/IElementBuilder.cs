namespace KaiHeiLa;

public interface IElementBuilder
{
    ElementType Type { get; }
    
    IElement Build();
}