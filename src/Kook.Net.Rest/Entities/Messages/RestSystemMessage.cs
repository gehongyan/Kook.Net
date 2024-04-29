using System.Diagnostics;
using Kook.API;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based system message.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestSystemMessage : RestMessage, ISystemMessage
{
    /// <inheritdoc />
    public SystemMessageType SystemMessageType { get; private set; }

    internal RestSystemMessage(BaseKookClient kook, Guid id, MessageType messageType, IMessageChannel channel, IUser author)
        : base(kook, id, messageType, channel, author, MessageSource.System)
    {
    }

    internal static new RestSystemMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, Message model)
    {
        RestSystemMessage entity = new(kook, model.Id, model.Type, channel, author);
        entity.Update(model);
        return entity;
    }

    internal static new RestSystemMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, API.DirectMessage model)
    {
        RestSystemMessage entity = new(kook, model.Id, model.Type, channel, author);
        entity.Update(model);
        return entity;
    }

    internal override void Update(Message model) => base.Update(model);

    // TODO: SystemMessageType
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
}
