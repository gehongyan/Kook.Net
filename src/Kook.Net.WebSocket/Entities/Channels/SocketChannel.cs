using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based channel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class SocketChannel : SocketEntity<ulong>, IChannel, IUpdateable
{
    #region SocketChannel

    /// <summary>
    ///     Gets a collection of users from the WebSocket cache.
    /// </summary>
    public IReadOnlyCollection<SocketUser> Users => GetUsersInternal();

    internal SocketChannel(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal abstract void Update(ClientState state, Model model);

    /// <inheritdoc />
    public abstract Task UpdateAsync(RequestOptions? options = null);

    #endregion

    #region User

    /// <summary>
    ///     Gets a generic user from this channel.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A generic WebSocket-based user associated with the identifier.
    /// </returns>
    public SocketUser? GetUser(ulong id) => GetUserInternal(id);

    internal abstract SocketUser? GetUserInternal(ulong id);
    internal abstract IReadOnlyCollection<SocketUser> GetUsersInternal();

    #endregion

    private string DebuggerDisplay => $"Unknown ({Id}, Channel)";
    internal SocketChannel Clone() => (SocketChannel)MemberwiseClone();

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => string.Empty;

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
        => Task.FromResult<IUser?>(null); //Overridden

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden

    #endregion
}
