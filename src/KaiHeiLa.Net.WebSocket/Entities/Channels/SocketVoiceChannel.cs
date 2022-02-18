using System.Diagnostics;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based voice channel in a guild.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketVoiceChannel : SocketGuildChannel, IVoiceChannel, ISocketAudioChannel
{
    #region SocketVoiceChannel

    /// <inheritdoc />
    public ulong? CategoryId { get; set; }
    /// <inheritdoc />
    public string Mention => MentionUtils.MentionChannel(Id);
    /// <inheritdoc />
    public bool IsPermissionSynced { get; set; }

    public VoiceQuality VoiceQuality { get; set; }
    
    public int? UserLimit { get; set; }
    
    public string ServerUrl { get; set; }

    internal SocketVoiceChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild) 
        : base(kaiHeiLa, id, guild)
    {
        Type = ChannelType.Voice;
    }
    internal new static SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketVoiceChannel(guild.KaiHeiLa, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId;
        VoiceQuality = model.VoiceQuality ?? VoiceQuality.Unspecified;
        UserLimit = model.UserLimit ?? 0;
        ServerUrl = model.ServerUrl;
        IsPermissionSynced = model.PermissionSync == 1;
    }
    
    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;
}