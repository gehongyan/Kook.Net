using Model = Kook.API.Reaction;

namespace Kook.Rest;

/// <summary>
///     Represents a single REST-based reaction.
/// </summary>
public class RestReaction : IReaction
{
    /// <inheritdoc />
    public IEmote Emote { get; }

    /// <summary>
    ///     Gets the number of reactions added.
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     Gets whether the reactions is added by the user.
    /// </summary>
    public bool Me { get; }

    internal RestReaction(IEmote emote, int count, bool me)
    {
        Emote = emote;
        Count = count;
        Me = me;
    }

    internal static RestReaction Create(Model model)
    {
        IEmote emote;
        if (Emoji.TryParse(model.Emoji.Id, out Emoji emoji))
            emote = emoji;
        else
            emote = new Emote(model.Emoji.Id, model.Emoji.Name);

        return new RestReaction(emote, model.Count, model.IsMe);
    }
}
