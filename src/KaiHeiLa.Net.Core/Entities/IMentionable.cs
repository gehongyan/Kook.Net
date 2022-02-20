namespace KaiHeiLa;

public interface IMentionable
{
    string KMarkdownMention { get; }
    
    string PlainTextMention { get; }
}