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
    
    /// <summary>
    ///     Gets a collection of users from the WebSocket cache.
    /// </summary>
    public IReadOnlyCollection<SocketUser> Users => GetUsersInternal();
    
    internal SocketChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id) 
        : base(kaiHeiLa, id)
    {
    }
    
    internal abstract void Update(ClientState state, Model model);
    
    #endregion

    #region User
    
    /// <summary>
    ///     Gets a generic user from this channel.
    /// </summary>
    /// <param name="id">The snowflake identifier of the user.</param>
    /// <returns>
    ///     A generic WebSocket-based user associated with the snowflake identifier.
    /// </returns>
    public SocketUser GetUser(ulong id) => GetUserInternal(id);
    internal abstract SocketUser GetUserInternal(ulong id);
    internal abstract IReadOnlyCollection<SocketUser> GetUsersInternal();

    #endregion
    
    private string DebuggerDisplay => $"Unknown ({Id}, Channel)";
    internal SocketChannel Clone() => MemberwiseClone() as SocketChannel;
    
    #region IChannel

    string IChannel.Name => null;
    uint IChannel.CreateUserId => default;
    /// <inheritdoc />
    /// 
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null); //Overridden

    #endregion
}