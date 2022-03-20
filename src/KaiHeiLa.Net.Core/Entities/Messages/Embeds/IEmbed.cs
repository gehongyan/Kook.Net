namespace KaiHeiLa;

public interface IEmbed
{
    EmbedType Type { get; }
    
    string Url { get; }
}