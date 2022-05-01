using Model = KaiHeiLa.API.Reaction;

namespace KaiHeiLa.Rest;

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
        if (model.Emoji.Id != model.Emoji.Name)
            emote = new Emote(model.Emoji.Id, model.Emoji.Name);
        else
            emote = new Emoji(model.Emoji.Name);
        return new RestReaction(emote, model.Count, model.IsMe);
    }
}