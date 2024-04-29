using System.Diagnostics.CodeAnalysis;
using Kook.API.Gateway;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based reaction.
/// </summary>
public class SocketReaction : IReaction
{
    /// <summary>
    ///     Gets the ID of the user who added the reaction.
    /// </summary>
    /// <remarks>
    ///     This property retrieves the identifier of the user responsible for this reaction. This
    ///     property will always contain the user identifier in event that
    ///     <see cref="Kook.WebSocket.SocketReaction.User" /> cannot be retrieved.
    /// </remarks>
    /// <returns>
    ///     A user identifier associated with the user.
    /// </returns>
    public ulong UserId { get; }

    /// <summary>
    ///     Gets the user who added the reaction if possible.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property attempts to retrieve a WebSocket-cached user that is responsible for this reaction from
    ///         the client. In other words, when the user is not in the WebSocket cache, this property may not
    ///         contain a value, leaving the only identifiable information to be
    ///         <see cref="Kook.WebSocket.SocketReaction.UserId" />.
    ///     </para>
    ///     <para>
    ///         If you wish to obtain an identifiable user object, consider utilizing
    ///         <see cref="Kook.Rest.KookRestClient" /> which will attempt to retrieve the user from REST.
    ///     </para>
    /// </remarks>
    /// <returns>
    ///     A user object where possible; a value is not always returned.
    /// </returns>
    public IUser? User { get; internal set; }

    /// <summary>
    ///     Gets the ID of the message that has been reacted to.
    /// </summary>
    /// <returns>
    ///     A message Guid associated with the message.
    /// </returns>
    public Guid MessageId { get; }

    /// <summary>
    ///     Gets the message that has been reacted to if possible.
    /// </summary>
    /// <returns>
    ///     A WebSocket-based message where possible; a value is not always returned.
    /// </returns>
    public IMessage? Message { get; internal set; }

    /// <summary>
    ///     Gets the channel where the reaction takes place in.
    /// </summary>
    /// <returns>
    ///     A WebSocket-based message channel.
    /// </returns>
    public ISocketMessageChannel? Channel { get; }

    /// <inheritdoc />
    public IEmote Emote { get; }

    internal SocketReaction(ISocketMessageChannel? channel, Guid messageId,
        IMessage? message, ulong userId, IUser? user, IEmote emoji)
    {
        Channel = channel;
        MessageId = messageId;
        Message = message;
        UserId = userId;
        User = user;
        Emote = emoji;
    }

    internal static SocketReaction Create(Reaction model, ISocketMessageChannel? channel,
        IMessage? message, IUser? user)
    {
        IEmote emote = Emoji.TryParse(model.Emoji.Id, out Emoji? emoji)
            ? emoji
            : new Emote(model.Emoji.Id, model.Emoji.Name);
        return new SocketReaction(channel, model.MessageId, message, model.UserId, user, emote);
    }

    internal static SocketReaction Create(PrivateReaction model, ISocketMessageChannel? channel,
        IMessage? message, IUser? user)
    {
        IEmote emote = Emoji.TryParse(model.Emoji.Id, out Emoji? emoji)
            ? emoji
            : new Emote(model.Emoji.Id, model.Emoji.Name);
        return new SocketReaction(channel, model.MessageId, message, model.UserId, user, emote);
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null) return false;
        if (obj == this) return true;
        if (obj is not SocketReaction otherReaction) return false;
        return UserId == otherReaction.UserId
            && MessageId == otherReaction.MessageId
            && Emote.Equals(otherReaction.Emote);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = UserId.GetHashCode();
            hashCode = (hashCode * 397) ^ MessageId.GetHashCode();
            hashCode = (hashCode * 397) ^ Emote.GetHashCode();
            return hashCode;
        }
    }
}
