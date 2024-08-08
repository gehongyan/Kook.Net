using Model = Kook.API.Reaction;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的回应。
/// </summary>
public class RestReaction : IReaction
{
    /// <inheritdoc />
    public IEmote Emote { get; }

    /// <summary>
    ///     获取添加或跟随此回应的用户数量。
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     获取当前用户是否添加或跟随了此回应。
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
        IEmote emote = Emoji.TryParse(model.Emoji.Id, out Emoji? emoji)
            ? emoji
            : new Emote(model.Emoji.Id, model.Emoji.Name);
        return new RestReaction(emote, model.Count, model.IsMe);
    }
}
