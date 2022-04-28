namespace KaiHeiLa;

public interface IInvite : IEntity<uint>, IDeletable
{
    /// <summary>
    ///     Gets the unique identifier for this invite.
    /// </summary>
    /// <returns>
    ///     A string containing the invite code (e.g. <c>wEAF5t</c>).
    /// </returns>
    string Code { get; }
    /// <summary>
    ///     Gets the URL used to accept this invite using <see cref="Code"/>.
    /// </summary>
    /// <returns>
    ///     A string containing the full invite URL (e.g. <c>https://kaihei.co/wEAF5t</c>).
    /// </returns>
    string Url { get; }
    
    /// <summary>
    ///     Gets the user that created this invite.
    /// </summary>
    /// <returns>
    ///     A user that created this invite.
    /// </returns>
    IUser Inviter { get; }
    /// <summary>
    ///     Gets the channel this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A generic channel that the invite points to.
    /// </returns>
    IChannel Channel { get; }
    /// <summary>
    ///     Gets the type of the channel this invite is linked to.
    /// </summary>
    ChannelType ChannelType { get; }
    /// <summary>
    ///     Gets the ID of the channel this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A ulong representing the channel snowflake identifier that the invite points to.
    /// </returns>
    ulong? ChannelId { get; }
    /// <summary>
    ///     Gets the name of the channel this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the channel that the invite points to.
    /// </returns>
    string ChannelName { get; }

    /// <summary>
    ///     Gets the guild this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A guild object representing the guild that the invite points to.
    /// </returns>
    IGuild Guild { get; }
    /// <summary>
    ///     Gets the ID of the guild this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A ulong representing the guild snowflake identifier that the invite points to.
    /// </returns>
    ulong? GuildId { get; }
    /// <summary>
    ///     Gets the name of the guild this invite is linked to.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the guild that the invite points to.
    /// </returns>
    string GuildName { get; }
    
    /// <summary>
    ///     Gets the time at which this invite will expire.
    /// </summary>
    /// <returns>
    ///     An <see cref="DateTimeOffset"/> representing the time until this invite expires; <c>null</c> if this
    ///     invite never expires.
    /// </returns>
    DateTimeOffset? ExpiresAt { get; }
    /// <summary>
    ///     Gets the time span until the invite expires.
    /// </summary>
    /// <returns>
    ///     An <see cref="TimeSpan"/> representing the time span until this invite expires; <c>null</c> if this
    ///     invite never expires.
    /// </returns>
    TimeSpan? MaxAge { get; }
    /// <summary>
    ///     Gets the max number of uses this invite may have.
    /// </summary>
    /// <returns>
    ///     An int representing the number of uses this invite may be accepted until it is removed
    ///     from the guild; <c>null</c> if none is set.
    /// </returns>
    int? MaxUses { get; }
    /// <summary>
    ///     Gets the number of times this invite has been used.
    /// </summary>
    /// <returns>
    ///     An int representing the number of times this invite has been used; <c>null</c> if none is set.
    /// </returns>
    int? Uses { get; }
    /// <summary>
    ///     Gets the number of times this invite still remains.
    /// </summary>
    /// <returns>
    ///     An int representing the number of times this invite still remains; <c>null</c> if none is set.
    /// </returns>
    int? RemainingUses { get; }
}