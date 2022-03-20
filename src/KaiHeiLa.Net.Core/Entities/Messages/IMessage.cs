namespace KaiHeiLa;

public interface IMessage : IGuidEntity, IDeletable
{
    #region General

    MessageType Type { get; }
    
    MessageSource Source { get; }
    
    bool? IsPinned { get; }
    
    IMessageChannel Channel { get; }
    
    IUser Author { get; }
    
    string Content { get; }
    
    string CleanContent { get; }
    
    DateTimeOffset Timestamp { get; }
    
    DateTimeOffset? EditedTimestamp { get; }
    
    IReadOnlyCollection<ulong> MentionedUserIds { get; }
    
    IReadOnlyCollection<uint> MentionedRoleIds { get; }
    
    bool? MentionedEveryone { get; }
    
    bool? MentionedHere { get; }
    
    IReadOnlyCollection<ITag> Tags { get; }
    
    IAttachment Attachment { get; }
    
    IReadOnlyCollection<ICard> Cards { get; }
    
    #endregion

    #region Reactions
    
    /// <summary>
    ///     Adds a reaction to this message.
    /// </summary>
    /// <param name="emote">The emoji used to react to this message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding a reaction to this message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    Task AddReactionAsync(IEmote emote, RequestOptions options = null);
    /// <summary>
    ///     Removes a reaction from message.
    /// </summary>
    /// <param name="emote">The emoji used to react to this message.</param>
    /// <param name="user">The user that added the emoji.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for removing a reaction to this message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null);
    /// <summary>
    ///     Removes a reaction from message.
    /// </summary>
    /// <param name="emote">The emoji used to react to this message.</param>
    /// <param name="userId">The ID of the user that added the emoji.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for removing a reaction to this message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null);
    
    /// <summary>
    ///     Gets all users that reacted to a message with a given emote.
    /// </summary>
    /// <param name="emote">The emoji that represents the reaction that you wish to get.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///      Collection of users.
    /// </returns>
    Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, RequestOptions options = null);
    
    #endregion
}