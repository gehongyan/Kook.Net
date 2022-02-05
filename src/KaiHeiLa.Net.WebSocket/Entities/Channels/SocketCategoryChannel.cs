using System.Diagnostics;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based category channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
{
    #region SocketCategoryChannel

    internal SocketCategoryChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild)
        : base(kaiHeiLa, id, guild)
    {
        Type = ChannelType.Category;
    }
    internal new static SocketCategoryChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketCategoryChannel(guild.KaiHeiLa, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    
    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Category)";
}