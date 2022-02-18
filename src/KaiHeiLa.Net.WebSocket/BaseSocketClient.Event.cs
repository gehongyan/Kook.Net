using KaiHeiLa.API;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public abstract partial class BaseSocketClient
{
    #region Messages

    /// <summary> Fired when a message is received. </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when a message is received. The event handler must return a
    ///         <see cref="Task"/> and accept a <see cref="SocketMessage"/> as its parameter.
    ///     </para>
    ///     <para>
    ///         The message that is sent to the client is passed into the event handler parameter as
    ///         <see cref="SocketMessage"/>. This message may be a system message (i.e.
    ///         <see cref="SocketSystemMessage"/>) or a user message (i.e. <see cref="SocketUserMessage"/>. See the
    ///         derived classes of <see cref="SocketMessage"/> for more details.
    ///     </para>
    /// </remarks>
    public event Func<SocketMessage, Task> MessageReceived
    {
        add { _messageReceivedEvent.Add(value); }
        remove { _messageReceivedEvent.Remove(value); }
    }
    internal readonly AsyncEvent<Func<SocketMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<SocketMessage, Task>>();

    #endregion
}