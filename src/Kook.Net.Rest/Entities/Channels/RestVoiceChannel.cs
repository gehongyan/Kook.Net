using Model = Kook.API.Channel;

using System.Diagnostics;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based voice channel in a guild.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestVoiceChannel : RestGuildChannel, IVoiceChannel, IRestAudioChannel
{
    #region RestVoiceChannel

    /// <inheritdoc />
    public VoiceQuality? VoiceQuality { get; private set; }
    /// <inheritdoc />
    public int? UserLimit { get; private set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <inheritdoc />
    public string ServerUrl { get; private set; }
    /// <inheritdoc />
    public bool HasPassword { get; private set; }
    /// <inheritdoc />
    public bool IsPermissionSynced { get; private set; }
    /// <inheritdoc />
    public ulong CreatorId { get; private set; }
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    internal RestVoiceChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id, ChannelType.Voice)
    {
    }
    
    internal new static RestVoiceChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        var entity = new RestVoiceChannel(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        VoiceQuality = model.VoiceQuality;
        UserLimit = model.UserLimit;
        ServerUrl = model.ServerUrl;
        IsPermissionSynced = model.PermissionSync == 1;
        HasPassword = model.HasPassword;
        CreatorId = model.CreatorId;
    }
    
    /// <inheritdoc />
    public async Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions options = null)
    {
        var model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }
    
    /// <summary>
    ///     Gets the parent (category) channel of this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the category channel
    ///     representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    public Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
        => ChannelHelper.GetCategoryAsync(this, Kook, options);

    /// <summary>
    ///     Gets the owner of this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the owner of this guild.
    /// </returns>
    public Task<RestUser> GetCreatorAsync(RequestOptions options = null)
        => ClientHelper.GetUserAsync(Kook, CreatorId, options);

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
    
    
    #region IGuildChannel
    
    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(null);
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
    
    #endregion
    
    #region INestedChannel
    
    /// <inheritdoc />
    async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
    {
        if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
            return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
        return null;
    }
    /// <inheritdoc />
    async Task<IUser> INestedChannel.GetCreatorAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetCreatorAsync(options).ConfigureAwait(false);
        else
            return null;
    }
    
    #endregion
    
}