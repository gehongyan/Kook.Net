using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的频道。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class SocketChannel : SocketEntity<ulong>, IChannel, IUpdateable
{
    #region SocketChannel

    /// <summary>
    ///     获取用户缓存列表中的可以访问此频道的所有用户。
    /// </summary>
    /// <remarks>
    ///     此属性仅会返回缓存中可以访问此频道的所有用户，如果未启用用户列表缓存，或者由于网关事件确实导致本地缓存不同步，此属性所返回的用户列表可能不准确。
    /// </remarks>
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
    ///     获取此频道中的一个用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <returns> 如果找到了具有指定 ID 的用户，则返回该用户；否则返回 <c>null</c>。 </returns>
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
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(null); //Overridden

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden

    #endregion
}
