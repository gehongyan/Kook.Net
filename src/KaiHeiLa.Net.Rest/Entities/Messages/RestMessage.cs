using System.Collections.Immutable;
using Model = KaiHeiLa.API.Rest.CreateMessageResponse;

namespace KaiHeiLa.Rest;

// TODO: RestMessage
/// <summary>
///     Represents a REST-based message.
/// </summary>
public abstract class RestMessage : RestEntity<Guid>, IMessage //, IUpdateable
{
    public ChannelType ChannelType { get; }
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

    public DateTimeOffset Timestamp { get; private set; }
    /// <inheritdoc />
    public virtual bool? MentionedEveryone => false;
    /// <inheritdoc />
    public virtual bool? MentionedHere => false;
    
    /// <summary>
    ///     Gets the <see cref="Attachment"/> on the message.
    /// </summary>
    public virtual IAttachment Attachment => null;
    /// <summary>
    ///     Gets a collection of the <see cref="ICard"/>'s on the message.
    /// </summary>
    public virtual IReadOnlyCollection<ICard> Cards => ImmutableArray.Create<ICard>();
    
    /// <inheritdoc />
    public virtual IReadOnlyCollection<uint> MentionedRoleIds => ImmutableArray.Create<uint>();
    /// <inheritdoc />
    public virtual IReadOnlyCollection<ulong> MentionedUserIds { get; private set; }
    
    internal RestMessage(BaseKaiHeiLaClient kaiHeiLa, Guid id, MessageType messageType, ChannelType channelType,
        IMessageChannel channel, IUser author, MessageSource source)
        : base(kaiHeiLa, id)
    {
        Type = messageType;
        ChannelType = channelType;
        Channel = channel;
        Author = author;
        Source = source;
    }
    // internal static RestMessage Create(BaseKaiHeiLaClient kaiHeiLa, IMessageChannel channel, IUser author, Model model)
    // {
    //
    //     if (model.Type == MessageType.Default ||
    //         model.Type == MessageType.Reply ||
    //         model.Type == MessageType.ApplicationCommand ||
    //         model.Type == MessageType.ThreadStarterMessage)
    //         return RestUserMessage.Create(kaiHeiLa, channel, author, model);
    //     else
    //         return RestSystemMessage.Create(kaiHeiLa, channel, author, model);
    // }
    
    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => MessageHelper.DeleteAsync(this, KaiHeiLa, options);
}