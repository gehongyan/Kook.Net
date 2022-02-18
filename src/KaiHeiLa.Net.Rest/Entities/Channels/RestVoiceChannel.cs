using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based voice channel in a guild.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestVoiceChannel : RestGuildChannel, IVoiceChannel, IRestAudioChannel
{
    #region RestVoiceChannel

    /// <inheritdoc />
    public VoiceQuality VoiceQuality { get; private set; }
    /// <inheritdoc />
    public int? UserLimit { get; private set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <inheritdoc />
    public string ServerUrl { get; private set; }
    /// <inheritdoc />
    public bool IsPermissionSynced { get; private set; }
    
    public string Mention => MentionUtils.MentionChannel(Id);

    internal RestVoiceChannel(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id)
        : base(kaiHeiLa, guild, id, ChannelType.Voice)
    {
    }
    
    internal new static RestVoiceChannel Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        var entity = new RestVoiceChannel(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId;
        VoiceQuality = model.VoiceQuality ?? VoiceQuality.Unspecified;
        UserLimit = model.UserLimit;
        ServerUrl = model.ServerUrl;
        IsPermissionSynced = model.PermissionSync == 1;
    }
    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
}