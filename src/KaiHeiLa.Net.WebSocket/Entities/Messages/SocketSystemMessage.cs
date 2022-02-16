using System.Diagnostics;
using KaiHeiLa.API.Gateway;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based message sent by the system.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketSystemMessage : SocketMessage, ISystemMessage
{
    public SystemMessageType SystemMessageType { get; private set; }
    
    internal SocketSystemMessage(KaiHeiLaSocketClient discord, Guid id, ISocketMessageChannel channel, SocketUser author)
        : base(discord, id, channel, author, MessageSource.System)
    {
    }
    internal new static SocketSystemMessage Create(KaiHeiLaSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketSystemMessage(discord, gatewayEvent.MessageId, channel, author);
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal new static SocketSystemMessage Create(KaiHeiLaSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketSystemMessage(discord, gatewayEvent.MessageId, channel, author);
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal override void Update(ClientState state, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        // TODO: SystemMessageType
    }
    internal override void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        // TODO: SystemMessageType
    }
        
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
    internal new SocketSystemMessage Clone() => MemberwiseClone() as SocketSystemMessage;
}