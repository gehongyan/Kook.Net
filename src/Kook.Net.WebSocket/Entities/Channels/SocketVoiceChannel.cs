using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Rest;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

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
    public bool IsPermissionSynced { get; private set; }
    /// <inheritdoc />
    public ulong CreatorId { get; private set; }
    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <remarks>
    ///     This method will try to get the user as a member of this channel. If the user is not a member of this guild,
    ///     this method will return <c>null</c>. To get the creator under this circumstance, use
    ///     <see cref="Kook.Rest.KookRestClient.GetUserAsync(ulong,RequestOptions)"/>.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    public SocketGuildUser Creator => GetUser(CreatorId);
    /// <inheritdoc />
    public VoiceQuality? VoiceQuality { get; set; }
    /// <inheritdoc />
    public int? UserLimit { get; set; }
    /// <inheritdoc />
    public string ServerUrl { get; set; }
    /// <inheritdoc />
    public bool HasPassword { get; set; }
    
    // /// <summary>
    // ///     Gets a collection of users that are able to connect to this voice channel.
    // /// </summary>
    // /// <returns>
    // ///     A read-only collection of users that are able to connect to this voice channel.
    // /// </returns>
    // public override IReadOnlyCollection<SocketGuildUser> Users
    //     => Guild.Users.Where(x => Permissions.GetValue(
    //         Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
    //         ChannelPermission.ViewChannel | ChannelPermission.Connect)).ToImmutableArray();
    
    internal SocketVoiceChannel(KookSocketClient kook, ulong id, SocketGuild guild) 
        : base(kook, id, guild)
    {
        Type = ChannelType.Voice;
    }
    internal new static SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketVoiceChannel(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        VoiceQuality = model.VoiceQuality;
        UserLimit = model.UserLimit ?? 0;
        ServerUrl = model.ServerUrl;
        IsPermissionSynced = model.PermissionSync == 1;
        HasPassword = model.HasPassword;
        CreatorId = model.CreatorId;
    }
    
    /// <inheritdoc />
    public Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions options = null)
        => ChannelHelper.ModifyAsync(this, Kook, func, options);

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
        => await ChannelHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

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
    
    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <remarks>
    ///     This method will try to get the user as a member of this channel. If the user is not a member of this guild,
    ///     this method will return <c>null</c>. To get the creator under this circumstance, use
    ///     <see cref="Kook.Rest.KookRestClient.GetUserAsync(ulong,RequestOptions)"/>.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    Task<IUser> INestedChannel.GetCreatorAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(Creator);

    #endregion
}