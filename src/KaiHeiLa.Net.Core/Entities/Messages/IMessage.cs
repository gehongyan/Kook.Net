namespace KaiHeiLa;

/// <summary>
///     Represents a message object.
/// </summary>
public interface IMessage : IEntity<Guid>, IDeletable
{
    #region General

    /// <summary>
    ///     Gets the type of this message.
    /// </summary>
    MessageType Type { get; }
    
    /// <summary>
    ///     Gets the source type of this message.
    /// </summary>
    MessageSource Source { get; }
    
    /// <summary>
    ///     Gets the value that indicates whether this message is pinned.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this message was added to its channel's pinned messages; otherwise <c>false</c>.
    /// </returns>
    bool? IsPinned { get; }
    
    /// <summary>
    ///     Gets the source channel of the message.
    /// </summary>
    IMessageChannel Channel { get; }
    
    /// <summary>
    ///     Gets the author of this message.
    /// </summary>
    IUser Author { get; }
    
    /// <summary>
    ///     Gets the content for this message.
    /// </summary>
    /// <returns>
    ///     A string that contains the body of the message;
    ///     note that this field may be empty or the original code if the message is not a text based message.
    /// </returns>
    string Content { get; }
    
    /// <summary>
    ///     Gets the clean content for this message.
    /// </summary>
    /// <returns>
    ///     A string that contains the body of the message stripped of mentions, markdown, emojis and pings;
    ///     note that this field may be empty or the original code if the message is not a text based message.
    /// </returns>
    string CleanContent { get; }
    
    /// <summary>
    ///     Gets the time this message was sent.
    /// </summary>
    /// <returns>
    ///     Time of when the message was sent.
    /// </returns>
    DateTimeOffset Timestamp { get; }
    
    /// <summary>
    ///     Gets the time of this message's last edit.
    /// </summary>
    /// <returns>
    ///     Time of when the message was last edited;
    ///     <c>null</c> if the message is never edited.
    /// </returns>
    DateTimeOffset? EditedTimestamp { get; }
    
    /// <summary>
    ///     Gets the IDs of users mentioned in this message.
    /// </summary>
    /// <returns>
    ///     A read-only collection of user IDs.
    /// </returns>
    IReadOnlyCollection<ulong> MentionedUserIds { get; }
    
    /// <summary>
    ///     Gets the IDs of roles mentioned in this message.
    /// </summary>
    /// <returns>
    ///     A read-only collection of role IDs.
    /// </returns>
    IReadOnlyCollection<uint> MentionedRoleIds { get; }
    
    /// <summary>
    ///     Gets the value that indicates whether this message mentioned everyone.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this message mentioned everyone; otherwise <c>false</c>.
    /// </returns>
    bool? MentionedEveryone { get; }
    
    /// <summary>
    ///     Gets the value that indicates whether this message mentioned online users.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this message mentioned online users; otherwise <c>false</c>.
    /// </returns>
    bool? MentionedHere { get; }
    
    /// <summary>
    ///     Gets all tags included in this message's content.
    /// </summary>
    IReadOnlyCollection<ITag> Tags { get; }
    
    /// <summary>
    ///     Gets the attachment included in this message.
    /// </summary>
    /// <returns>
    ///     The attachment included in this message;
    /// </returns>
    IAttachment Attachment { get; }
    
    /// <summary>
    ///     Gets all cards included in this message.
    /// </summary>
    /// <returns>
    ///     A read-only collection of card objects.
    /// </returns>
    IReadOnlyCollection<ICard> Cards { get; }
    
    /// <summary>
    ///     Gets all embeds included in this message.
    /// </summary>
    /// <returns>
    ///     A read-only collection of embed objects.
    /// </returns>
    IReadOnlyCollection<IEmbed> Embeds { get; }
    
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