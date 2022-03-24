using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.Rest;
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
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     An <see cref="ICategoryChannel"/> representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    public ICategoryChannel Category
        => CategoryId.HasValue ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel : null;
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

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
    
    // /// <inheritdoc />
    // public override SocketGuildUser GetUser(ulong id)
    // {
    //     var user = Guild.GetUser(id);
    //     if (user?.VoiceChannel?.Id == Id)
    //         return user;
    //     return null;
    // }
    
    #endregion
    
    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null)
        => await ChannelHelper.GetInvitesAsync(this, KaiHeiLa, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, KaiHeiLa, maxAge, maxUses, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge.OneWeek, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, KaiHeiLa, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;
    
    #region IGuildChannel
    
    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(GetUser(id));
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    
    #endregion
    
    #region INestedChannel
    
    /// <inheritdoc />
    Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult(Category);
    
    #endregion
}