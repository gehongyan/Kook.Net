using System.Diagnostics;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class SocketChannel : SocketEntity<ulong>, IChannel, IUpdateable
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
    
    /// <inheritdoc />
    public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);
    
    #endregion

    #region User
    
    /// <summary>
    ///     Gets a generic user from this channel.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A generic WebSocket-based user associated with the identifier.
    /// </returns>
    public SocketUser GetUser(ulong id) => GetUserInternal(id);
    internal abstract SocketUser GetUserInternal(ulong id);
    internal abstract IReadOnlyCollection<SocketUser> GetUsersInternal();

    #endregion
    
    private string DebuggerDisplay => $"Unknown ({Id}, Channel)";
    internal SocketChannel Clone() => MemberwiseClone() as SocketChannel;
    
    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => null;
    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null); //Overridden
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden

    #endregion
}