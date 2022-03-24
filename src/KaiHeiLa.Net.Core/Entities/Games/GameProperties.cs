namespace KaiHeiLa;

/// <summary>
///     Properties that are used to modify an <see cref="IGame" /> with the specified changes.
/// </summary>
/// <seealso cref="IGame.ModifyAsync"/>
public class GameProperties
{
    public string Name { get; set; }
    public string IconUrl { get; set; }
}