namespace Kook.Net.Samples.CardMarkup.Models.Template;

public record Vote
{
    public required string Title { get; set; }

    public required List<Game> Games { get; set; }
}
