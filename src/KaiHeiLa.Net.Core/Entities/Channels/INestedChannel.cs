namespace KaiHeiLa;

/// <summary>
///     Represents a type of guild channel that can be nested within a category.
/// </summary>
public interface INestedChannel : IGuildChannel
{
    #region General

    /// <summary>
    ///     Gets the parent (category) ID of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong"/> representing the identifier of the parent of this channel;
    ///     <c>null</c> if none is set.
    /// </returns>
    ulong? CategoryId { get; }
    
    /// <summary>
    ///     Specifies whether the permissions of this channel is synchronized with its parent channel.
    /// </summary>
    /// <returns>
    ///     A <see langword="bool"/> indicating whether the permissions of this channel is synchronized with its parent channel.
    /// </returns>
    bool IsPermissionSynced { get; }
    
    /// <summary>
    ///     Gets the identifier of the user who created this channel.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong"/> representing the identifier of the user who created this channel.
    /// </returns>
    ulong CreatorId { get; }
    
    /// <summary>
    ///     Gets the parent (category) channel of this channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the category channel
    ///     representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    Task<IUser> GetCreatorAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    
    #endregion

    #region Invites

    /// <summary>
    ///     Gets a collection of all invites from this guild channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     invite, each representing information for an invite found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null);

    /// <summary>
    ///     Creates a new invite to this channel.
    /// </summary>
    /// <param name="maxAge">The time until the invite expires. Set to <c>InviteMaxAge.NeverExpires</c> to never expire.</param>
    /// <param name="maxUses">The max amount of times this invite may be used. Set to <c>InviteMaxUses.Unlimited</c> to have unlimited uses.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
    ///     metadata object containing information for the created invite.
    /// </returns>
    Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions options = null);
    
    /// <summary>
    ///     Creates a new invite to this channel.
    /// </summary>
    /// <param name="maxAge">The time (in seconds) until the invite expires. Set to <c>null</c> to never expire.</param>
    /// <param name="maxUses">The max amount of times this invite may be used. Set to <c>null</c> to have unlimited uses.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
    ///     metadata object containing information for the created invite.
    /// </returns>
    Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null);
    
    #endregion
}