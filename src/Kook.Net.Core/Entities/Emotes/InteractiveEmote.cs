using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个互动表情符号。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class InteractiveEmote : Emote
{
    /// <summary>
    ///     创建一个新的 <see cref="Kook.InteractiveEmote" /> 实例。
    /// </summary>
    internal InteractiveEmote(string id, string name, InteractiveEmoteType interactiveType)
        : base(id, name, EmojiType.Interactive)
    {
        InteractiveEmoteType = interactiveType;
    }

    /// <summary>
    ///     获取此互动表情符号的类型。
    /// </summary>
    public InteractiveEmoteType InteractiveEmoteType { get; }

    private string DebuggerDisplay => $"{Name} ({Id}{(Type.HasValue ? $", {Type}" : "")})";
}
