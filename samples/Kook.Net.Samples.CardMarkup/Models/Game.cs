namespace Kook.Net.Samples.CardMarkup.Models;

public class Game
{
    public required string Name { get; set; }

    public required string Slogan { get; set; }

    public required List<User> Voters { get; set; }
}
