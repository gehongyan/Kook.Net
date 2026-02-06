using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个包含结果的互动表情符号。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RolledInteractiveEmote : InteractiveEmote
{
    /// <summary>
    ///     使用指定的互动表情符号和随机结果集合初始化 <see cref="RolledInteractiveEmote"/> 类的新实例。
    /// </summary>
    internal RolledInteractiveEmote(InteractiveEmote emote, IReadOnlyCollection<InteractiveEmoteRollResult> rolls)
        : base(emote.Id, emote.Name, emote.InteractiveEmoteType, emote.DynamicImage)
    {
        Rolls = rolls;
    }

    /// <summary>
    ///     获取此互动表情符号的随机结果集合。
    /// </summary>
    public IReadOnlyCollection<InteractiveEmoteRollResult> Rolls { get; }

    /// <summary>
    ///     获取此互动表情符号的结果数量。
    /// </summary>
    public int ResultCount => Rolls.Count;

    private string DebuggerDisplay => $"{Name}, Rolls: {(Rolls.Count is 0 ? "None" : string.Join(", ", Rolls.Select(x => x.ToString())))} ({Id}{(Type.HasValue ? $", {Type}" : "")})";
}
