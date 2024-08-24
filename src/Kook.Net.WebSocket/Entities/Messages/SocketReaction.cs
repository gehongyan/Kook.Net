using System.Diagnostics.CodeAnalysis;
using Kook.API.Gateway;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的回应。
/// </summary>
public class SocketReaction : IReaction
{
    /// <summary>
    ///     获取添加此回应的用户的 ID。
    /// </summary>
    public ulong UserId { get; }

    /// <summary>
    ///     获取添加此回应的用户。
    /// </summary>
    /// <remarks>
    ///     如果要获取的用户实体不存在于缓存中，则此属性将返回 <see langword="null"/>。
    /// </remarks>
    public IUser? User { get; internal set; }

    /// <summary>
    ///     获取此回应所对应的消息的 ID。
    /// </summary>
    public Guid MessageId { get; }

    /// <summary>
    ///     获取此回应所对应的消息。
    /// </summary>
    /// <remarks>
    ///     如果要获取的消息实体不存在于缓存中，则此属性将返回 <see langword="null"/>。
    /// </remarks>
    public IMessage? Message { get; internal set; }

    /// <summary>
    ///     获取此回应所在的消息频道。
    /// </summary>
    /// <remarks>
    ///     如果要获取的频道实体不存在于缓存中，则此属性将返回 <see langword="null"/>。
    /// </remarks>
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
