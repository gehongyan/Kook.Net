using Kook.API.Gateway;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based message sent by the system.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketSystemMessage : SocketMessage, ISystemMessage
{
    /// <inheritdoc />
    public SystemMessageType SystemMessageType { get; private set; }

    internal SocketSystemMessage(KookSocketClient kook, Guid id, ISocketMessageChannel channel, SocketUser author)
        : base(kook, id, channel, author, MessageSource.System)
    {
    }

    internal static new SocketSystemMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        SocketSystemMessage entity = new(kook, gatewayEvent.MessageId, channel, author);
        entity.Update(state, model, gatewayEvent);
        return entity;
    }

    internal static new SocketSystemMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        SocketSystemMessage entity = new(kook, gatewayEvent.MessageId, channel, author);
        entity.Update(state, model, gatewayEvent);
        return entity;
    }

    internal static new SocketSystemMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        API.Message model)
    {
        SocketSystemMessage entity = new(kook, model.Id, channel, author);
        entity.Update(state, model);
        return entity;
    }

    internal static new SocketSystemMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        API.DirectMessage model)
    {
        SocketSystemMessage entity = new(kook, model.Id, channel, author);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent) =>
        base.Update(state, model, gatewayEvent);

    // TODO: SystemMessageType
    internal override void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent) =>
        base.Update(state, model, gatewayEvent);

    // TODO: SystemMessageType
    internal override void Update(ClientState state, API.Message model) => base.Update(state, model);

    // TODO: SystemMessageType
    internal override void Update(ClientState state, MessageUpdateEvent model) => base.Update(state, model);

    // TODO: SystemMessageType
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
    internal new SocketSystemMessage Clone() => MemberwiseClone() as SocketSystemMessage;
}
