using System.Diagnostics;
using Kook.API;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的系统消息。
/// </summary>
/// <remarks>
///     <note type="warning">
///         KOOK 未统一规范系统消息类型，此类并未在 <see cref="T:Kook.Rest.RestMessage"/> 之上封装更多的实用功能。
///     </note>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestSystemMessage : RestMessage, ISystemMessage
{
    /// <inheritdoc />
    public SystemMessageType SystemMessageType { get; private set; }

    internal RestSystemMessage(BaseKookClient kook, Guid id, MessageType messageType, IMessageChannel channel, IUser author)
        : base(kook, id, messageType, channel, author, MessageSource.System)
    {
        SystemMessageType = SystemMessageType.Unknown;
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
