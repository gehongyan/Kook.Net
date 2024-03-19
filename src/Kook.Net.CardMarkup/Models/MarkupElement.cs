namespace Kook.CardMarkup.Models;

internal record MarkupElement
{
    public string Name { get; set; }

    public string Text { get; set; }

    public Dictionary<string, string> Attributes { get; set; }

    public List<MarkupElement> Children { get; set; }
}
