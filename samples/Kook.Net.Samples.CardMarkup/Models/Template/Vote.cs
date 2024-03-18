namespace Kook.Net.Samples.CardMarkup.Models.Template;

public record Vote
{
    public string Title { get; set; }

    public List<Game> Games { get; set; }
}
