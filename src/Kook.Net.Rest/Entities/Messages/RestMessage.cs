using Kook.API;
using System.Collections.Immutable;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based message.
/// </summary>
public abstract class RestMessage : RestEntity<Guid>, IMessage, IUpdateable
{
    private ImmutableArray<RestReaction> _reactions = [];
    private ImmutableArray<RestUser> _userMentions = [];

    /// <inheritdoc />
    public MessageType Type { get; }

    /// <inheritdoc />
    public IMessageChannel Channel { get; }

    /// <summary>
    ///     Gets the Author of the message.
    /// </summary>
    public IUser Author { get; }

    /// <inheritdoc />
    public MessageSource Source { get; }

    /// <inheritdoc />
    public string Content { get; protected set; }

    /// <inheritdoc />
    public string CleanContent => MessageHelper.SanitizeMessage(this);

    /// <inheritdoc cref="IMessage.Attachments" />
    public virtual IReadOnlyCollection<Attachment> Attachments { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? EditedTimestamp { get; private set; }

    /// <inheritdoc />
    public virtual bool MentionedEveryone => false;

    /// <inheritdoc />
    public virtual bool MentionedHere => false;

    /// <summary>
    ///     Gets a collection of the <see cref="ICard"/>'s on the message.
    /// </summary>
    public virtual IReadOnlyCollection<ICard> Cards => ImmutableArray.Create<ICard>();

    /// <summary>
    ///     Gets a collection of the <see cref="IEmbed"/>'s on the message.
    /// </summary>
    public virtual IReadOnlyCollection<IEmbed> Embeds => ImmutableArray.Create<IEmbed>();

    /// <summary>
    ///     Gets a collection of the <see cref="RestPokeAction"/>'s on the message.
    /// </summary>
    public virtual IReadOnlyCollection<RestPokeAction> Pokes => ImmutableArray.Create<RestPokeAction>();

    /// <inheritdoc />
    public virtual IReadOnlyCollection<uint> MentionedRoleIds => ImmutableArray.Create<uint>();

    /// <summary>
    ///     Gets a collection of the mentioned users in the message.
    /// </summary>
    public IReadOnlyCollection<RestUser> MentionedUsers => _userMentions;

    /// <inheritdoc />
    public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();

    /// <inheritdoc />
    public virtual bool? IsPinned { get; internal set; }

    /// <summary>
    ///     Gets the <see cref="Content"/> of the message.
    /// </summary>
    /// <returns> A string that is the <see cref="Content"/> of the message. </returns>
    public override string ToString() => Content;

    internal RestMessage(BaseKookClient kook, Guid id, MessageType messageType,
        IMessageChannel channel, IUser author, MessageSource source)
        : base(kook, id)
    {
        Type = messageType;
        Channel = channel;
        Author = author;
        Source = source;
        Content = string.Empty;
        Attachments = [];
    }

    internal static RestMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, Message model)
    {
        return MessageHelper.GetSource(model) is MessageSource.System
            ? RestSystemMessage.Create(kook, channel, author, model)
            : RestUserMessage.Create(kook, channel, author, model);
    }

    internal static RestMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, DirectMessage model)
    {
        return MessageHelper.GetSource(author) is MessageSource.System
            ? RestSystemMessage.Create(kook, channel, author, model)
            : RestUserMessage.Create(kook, channel, author, model);
    }

    internal virtual void Update(Message model)
    {
        Timestamp = model.CreateAt;
        EditedTimestamp = model.UpdateAt;
        Content = model.Content;

        if (model.Reactions is not null)
            _reactions = [..model.Reactions.Select(RestReaction.Create)];

        if (model.MentionInfo?.MentionedUsers is not null)
            _userMentions = [..model.MentionInfo.MentionedUsers.Select(x => RestUser.Create(Kook, x))];
    }

    internal virtual void Update(DirectMessage model)
    {
        Timestamp = model.CreateAt;
        EditedTimestamp = model.UpdateAt;
        Content = model.Content;

        if (model.Reactions is not null)
            _reactions = [..model.Reactions.Select(RestReaction.Create)];

        if (model.MentionInfo?.MentionedUsers is not null)
            _userMentions = [..model.MentionInfo.MentionedUsers.Select(x => RestUser.Create(Kook, x))];
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions =>
        _reactions.ToDictionary(
            x => x.Emote,
            x => new ReactionMetadata
            {
                ReactionCount = x.Count,
                IsMe = x.Me
            });

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) => MessageHelper.DeleteAsync(this, Kook, options);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     This message is neither a guild channel message nor a direct message.
    /// </exception>
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        if (Channel is IGuildChannel)
        {
            Message model = await Kook.ApiClient.GetMessageAsync(Id, options).ConfigureAwait(false);
            Update(model);
            return;
        }

        if (Channel is IDMChannel dmChannel)
        {
            DirectMessage model = await Kook.ApiClient
                .GetDirectMessageAsync(Id, dmChannel.ChatCode, options)
                .ConfigureAwait(false);
            Update(model);
            return;
        }

        throw new NotSupportedException("The operation is not supported for this message type.");
    }

    /// <inheritdoc />
    public Task AddReactionAsync(IEmote emote, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.AddReactionAsync(this, emote, Kook, options),
            IDMChannel => MessageHelper.AddDirectMessageReactionAsync(this, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, user.Id, emote, Kook, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, user.Id, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, userId, emote, Kook, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, userId, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.GetReactionUsersAsync(this, emote, Kook, options),
            IDMChannel => MessageHelper.GetDirectMessageReactionUsersAsync(this, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    #region IMessage

    IUser IMessage.Author => Author;

    /// <inheritdoc />
    IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;

    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;

    /// <inheritdoc />
    IReadOnlyCollection<IPokeAction> IMessage.Pokes => Pokes;

    /// <inheritdoc />
    IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();

    #endregion
}
