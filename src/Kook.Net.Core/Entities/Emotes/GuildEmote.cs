using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个附属于服务器的基于图片的表情符号。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GuildEmote : Emote
{
    internal GuildEmote(string id, string name, bool? animated, ulong guildId, ulong? creatorId)
        : base(id, name, animated)
    {
        GuildId = guildId;
        CreatorId = creatorId;
    }

    /// <summary>
    ///     获取此表情符号所属的服务器的 ID。
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    ///     获取创建此表情符号的用户的 ID
    /// </summary>
    /// <remarks>
    ///     如果无法确定创建此表情符号的用户的 ID，则为 <c>null</c>。
    /// </remarks>
    public ulong? CreatorId { get; }

    private string DebuggerDisplay => $"{Name} ({Id}{(Animated == true ? ", Animated" : "")})";

    internal GuildEmote Clone() => (GuildEmote) MemberwiseClone();
}
