namespace Kook.CardMarkup.Models;

internal record MarkupElement
{
    public required string Name { get; set; }

    public string? Text { get; set; }

    public required Dictionary<string, string> Attributes { get; set; }

    public required List<MarkupElement> Children { get; set; }
}
