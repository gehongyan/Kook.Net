using System.Diagnostics;
using Model = Kook.API.VoiceRegion;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based voice region.
/// </summary>
/// <remarks>
///     <note type="warning">
///         This entity is still in experimental state, which means that it is not for official API implementation
///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
///     </note>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestVoiceRegion : RestEntity<string>, IVoiceRegion
{
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public decimal Crowding { get; private set; }

    internal RestVoiceRegion(BaseKookClient kook, string id)
        : base(kook, id)
    {
    }
    internal static RestVoiceRegion Create(BaseKookClient kook, Model model)
    {
        var entity = new RestVoiceRegion(kook, model.Id);
        entity.Update(model);
        return entity;
    }
    internal void Update(Model model)
    {
        Name = model.Name;
        Crowding = model.Crowding / 100M;
    }

    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id}, {Crowding:F2}%)";
}
