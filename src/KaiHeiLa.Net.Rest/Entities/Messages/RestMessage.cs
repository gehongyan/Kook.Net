using System.Collections.Immutable;
using Model = KaiHeiLa.API.Message;

namespace KaiHeiLa.Rest;

// TODO: RestMessage
/// <summary>
///     Represents a REST-based message.
/// </summary>
public abstract class RestMessage : RestEntity<Guid>, IMessage, IUpdateable
{
    private ImmutableArray<RestUser> _userMentions = ImmutableArray.Create<RestUser>();
    
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
    public string Content { get; private set; }

    public virtual Attachment Attachment { get; private set; }
    
    public DateTimeOffset Timestamp { get; private set; }
    /// <inheritdoc />
    public virtual bool? MentionedEveryone => false;
    /// <inheritdoc />
    public virtual bool? MentionedHere => false;

    /// <summary>
    ///     Gets the <see cref="Content"/> of the message.
    /// </summary>
    /// <returns>
    ///     A string that is the <see cref="Content"/> of the message.
    /// </returns>
    public override string ToString() => Content;
    
    IUser IMessage.Author => Author;
    /// <inheritdoc />
    IAttachment IMessage.Attachment => Attachment;
    /// <summary>
    ///     Gets a collection of the <see cref="ICard"/>'s on the message.
    /// </summary>
    public virtual IReadOnlyCollection<ICard> Cards => ImmutableArray.Create<ICard>();
    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;
    /// <inheritdoc />
    public virtual IReadOnlyCollection<uint> MentionedRoleIds => ImmutableArray.Create<uint>();
    /// <summary>
    ///     Gets a collection of the mentioned users in the message.
    /// </summary>
    public IReadOnlyCollection<RestUser> MentionedUsers => _userMentions;
    /// <inheritdoc />
    IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();
    
    internal RestMessage(BaseKaiHeiLaClient kaiHeiLa, Guid id, MessageType messageType, 
        IMessageChannel channel, IUser author, MessageSource source)
        : base(kaiHeiLa, id)
    {
        Type = messageType;
        Channel = channel;
        Author = author;
        Source = source;
    }
    internal static RestMessage Create(BaseKaiHeiLaClient kaiHeiLa, IMessageChannel channel, IUser author, Model model)
    {
        if (model.Type != MessageType.System)
            return RestUserMessage.Create(kaiHeiLa, channel, author, model);
        else
            return RestSystemMessage.Create(kaiHeiLa, channel, author, model);
    }

    internal virtual void Update(Model model)
    {
        Timestamp = model.CreateAt;
        Content = model.Content;
        if (model.MentionInfo?.MentionUsers is not null)
        {
            var value = model.MentionInfo.MentionUsers;
            if (value.Length > 0)
            {
                var newMentions = ImmutableArray.CreateBuilder<RestUser>(value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    var val = value[i];
                    if (val != null)
                        newMentions.Add(RestUser.Create(KaiHeiLa, val));
                }
                _userMentions = newMentions.ToImmutable();
            }
        }
    }
    
    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => MessageHelper.DeleteAsync(this, KaiHeiLa, options);
    
    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions options = null)
    {
        var model = await KaiHeiLa.ApiClient.GetMessageAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }
}