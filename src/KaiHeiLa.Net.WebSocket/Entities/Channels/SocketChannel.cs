using System.Diagnostics;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class SocketChannel : SocketEntity<ulong>, IChannel
{
    #region SocketChannel

    internal SocketChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id) 
        : base(kaiHeiLa, id)
    {
    }
    
    internal abstract void Update(ClientState state, Model model);
    
    #endregion

    private string DebuggerDisplay => $"Unknown ({Id}, Channel)";
    internal SocketChannel Clone() => MemberwiseClone() as SocketChannel;
    
    #region IChannel

    string IChannel.Name => null;
    uint IChannel.CreateUserId => default;

    #endregion
}