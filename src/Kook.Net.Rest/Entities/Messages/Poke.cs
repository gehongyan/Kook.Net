using System.Diagnostics;
using Model = Kook.API.Poke;

namespace Kook.Rest;

/// <summary>
///     Represents a poke in messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class Poke : IPoke
{
    /// <inheritdoc />
    public uint Id { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public TimeSpan Cooldown { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Categories { get; }

    /// <inheritdoc />
    public PokeLabel Label { get; }

    /// <inheritdoc />
    public PokeIcon Icon { get; }

    /// <inheritdoc />
    public PokeQuality Quality { get; }

    /// <inheritdoc />
    public IPokeResource Resource { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> MessageScenarios { get; }

    internal Poke(uint id, string name, string description, TimeSpan cooldown, IReadOnlyCollection<string> categories,
        PokeLabel label, PokeIcon icon, PokeQuality quality, IPokeResource resource,
        IReadOnlyDictionary<string, string> messageScenarios)
    {
        Id = id;
        Name = name;
        Description = description;
        Cooldown = cooldown;
        Categories = categories;
        Label = label;
        Icon = icon;
        Quality = quality;
        Resource = resource;
        MessageScenarios = messageScenarios;
    }

    internal static Poke Create(Model model)
    {
        TimeSpan cooldown = TimeSpan.FromSeconds(model.Cooldown);
        PokeLabel label = PokeLabel.Create(model.LabelId, model.LabelName);
        PokeIcon icon = PokeIcon.Create(model.Icon, model.IconExpired);
        PokeQuality quality = PokeQuality.Create(model.QualityId, model.Quality.Color, new Dictionary<string, string>()
        {
            ["small"] = model.Quality.Small,
            ["big"] = model.Quality.Big,
        });
        IPokeResource pokeResource = model.Resource.ToEntity();
        return new Poke(model.Id, model.Name, model.Description, cooldown, model.Categories, label,icon,
            quality, pokeResource, model.MessageScenarios);
    }
    
    /// <summary>
    ///     Returns the name of the poke.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the poke.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Resource.Type})";
}