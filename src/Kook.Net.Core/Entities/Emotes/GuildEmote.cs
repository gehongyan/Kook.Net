using System.Diagnostics;
using System.Globalization;

namespace Kook;

/// <summary>
///     An image-based emote that is attached to a guild.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class GuildEmote : Emote
{
    internal GuildEmote(string id, string name, bool? animated, ulong guildId, IUser creator) 
        : base(id, name, animated)
    {
        GuildId = guildId;
        Creator = creator;
    }

    /// <summary>
    ///     The ID of the guild this emote is attached to.
    /// </summary>
    /// <returns>
    ///     A ulong that identifies the guild this emote is attached to.
    /// </returns>
    public ulong GuildId { get; }
    
    /// <summary>
    ///     Gets the user who created this emote.
    /// </summary>
    /// <returns>
    ///     An <see cref="IUser"/> representing the user who created this emote.
    /// </returns>
    public IUser Creator { get; }
    
    private string DebuggerDisplay => $"{Name} ({Id}{(Animated == true ? ", Animated" : "")})";
    /// <summary>
    ///     Gets the raw representation of the emote.
    /// </summary>
    /// <returns>
    ///     A string representing the raw presentation of the emote.
    /// </returns>
    public override string ToString() => $"(emj){Name}(emj)[{Id}]";
}