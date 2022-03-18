using System.Diagnostics;
using Model = KaiHeiLa.API.Message;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based system message.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestSystemMessage : RestMessage, ISystemMessage
{
    public SystemMessageType SystemMessageType { get; set; }
    
    internal RestSystemMessage(BaseKaiHeiLaClient kaiHeiLa, Guid id, MessageType messageType, IMessageChannel channel, IUser author)
        : base(kaiHeiLa, id, messageType, channel, author, MessageSource.System)
    {
    }
    internal new static RestSystemMessage Create(BaseKaiHeiLaClient kaiHeiLa, IMessageChannel channel, IUser author, Model model)
    {
        var entity = new RestSystemMessage(kaiHeiLa, model.Id, model.Type, channel, author);
        entity.Update(model);
        return entity;
    }
    internal new static RestSystemMessage Create(BaseKaiHeiLaClient kaiHeiLa, IMessageChannel channel, IUser author, API.DirectMessage model)
    {
        var entity = new RestSystemMessage(kaiHeiLa, model.Id, model.Type, channel, author);
        entity.Update(model);
        return entity;
    }
    internal override void Update(Model model)
    {
        base.Update(model);
        // TODO: SystemMessageType
    }
    
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
}