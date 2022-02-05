using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based channel in a guild that can send and receive messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
{
    #region SocketTextChannel

    /// <inheritdoc />
    public string Topic { get; set; }
    /// <inheritdoc />
    public int SlowModeInterval { get; set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <inheritdoc />
    public bool IsPermissionSynced { get; set; }
    /// <inheritdoc />
    public string Mention => MentionUtils.MentionChannel(Id);
    
    internal SocketTextChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild)
        : base(kaiHeiLa, id, guild)
    {
        Type = ChannelType.Text;
    }
    internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketTextChannel(guild.KaiHeiLa, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode; // some guilds haven't been patched to include this yet?
    }
    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;

}