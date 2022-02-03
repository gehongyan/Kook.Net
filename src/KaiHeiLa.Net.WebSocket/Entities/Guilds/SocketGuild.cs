using System.Diagnostics;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based guild object.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketGuild : SocketEntity<ulong>, IGuild, IDisposable
{
    #region SocketGuild

    public string Name { get; private set; }
    public string Topic { get; private set; }
    public string MasterId { get; private set; }
    public string Icon { get; private set; }
    public NotifyType NotifyType { get; private set; }
    public string Region { get; private set; }
    public bool IsOpenEnabled { get; private set; }
    public string OpenId { get; private set; }
    public ulong DefaultChannelId { get; private set; }
    public ulong WelcomeChannelId { get; private set; }
    
    internal bool IsAvailable { get; private set; }
    /// <summary> Indicates whether the client is connected to this guild. </summary>
    public bool IsConnected { get; internal set; }

    internal SocketGuild(KaiHeiLaSocketClient kaiHeiLa, ulong id) : base(kaiHeiLa, id)
    {
    }
    #endregion
    
    /// <summary>
    ///     Gets the name of the guild.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="KaiHeiLa.WebSocket.SocketGuild.Name"/>.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id})";

    #region IGuild

    public void Dispose()
    {
    }

    #endregion
}