using System.Collections.Immutable;
using KaiHeiLa.API;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public class SocketDMChannel : SocketChannel, IDMChannel, ISocketPrivateChannel, ISocketMessageChannel
{
    #region SocketDMChannel

    public new Guid Id { get; set; }
    
    /// <summary>
    ///     Gets the recipient of the channel.
    /// </summary>
    public SocketUser Recipient { get; }
    
    /// <summary>
    ///     Gets a collection that is the current logged-in user and the recipient.
    /// </summary>
    public new IReadOnlyCollection<SocketUser> Users => ImmutableArray.Create(KaiHeiLa.CurrentUser, Recipient);

    internal SocketDMChannel(KaiHeiLaSocketClient kaiHeiLa, Guid chatCode, SocketUser recipient)
        : base(kaiHeiLa, default)
    {
        Id = chatCode;
        Recipient = recipient;
    }

    internal static SocketDMChannel Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, Guid chatCode, API.User recipient)
    {
        var entity = new SocketDMChannel(kaiHeiLa, chatCode, kaiHeiLa.GetOrCreateTemporaryUser(state, recipient));
        entity.Update(state, recipient);
        return entity;
    }
    internal void Update(ClientState state, API.User recipient)
    {
        Recipient.Update(state, recipient);
    }
    
    /// <inheritdoc />
    public Task CloseAsync(RequestOptions options = null)
        => ChannelHelper.DeleteDMChannelAsync(this, KaiHeiLa, options);
    
    #endregion

    #region Messages
    /// <inheritdoc />
    public SocketMessage GetCachedMessage(Guid id)
        => null;
    
    /// <summary>
    ///     Gets the message associated with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">TThe ID of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     The message gotten from either the cache or the download, or <c>null</c> if none is found.
    /// </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions options = null)
    {
        return await ChannelHelper.GetMessageAsync(this, KaiHeiLa, id, options).ConfigureAwait(false);
    }
    
    internal void AddMessage(SocketMessage msg)
    {
    }
    internal SocketMessage RemoveMessage(Guid id)
        => null;

    #endregion
    
    #region Users

    /// <summary>
    ///     Gets a user in this channel from the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A <see cref="SocketUser"/> object that is a recipient of this channel; otherwise <c>null</c>.
    /// </returns>
    public new SocketUser GetUser(ulong id)
    {
        if (id == Recipient.Id)
            return Recipient;
        else if (id == KaiHeiLa.CurrentUser.Id)
            return KaiHeiLa.CurrentUser;
        else
            return null;
    }

    #endregion

    #region SocketChannel

    /// <inheritdoc />
    internal override void Update(ClientState state, Channel model)
    {
        throw new NotSupportedException("Update a DMChannel via Channel is not supported");
    }
    
    /// <inheritdoc />
    internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;

    /// <inheritdoc />
    internal override SocketUser GetUserInternal(ulong id) => GetUser(id);
    #endregion
    
    #region IDMChannel
    /// <inheritdoc />
    IUser IDMChannel.Recipient => Recipient;
    #endregion

    #region ISocketPrivateChannel
    /// <inheritdoc />
    IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => ImmutableArray.Create(Recipient);
    #endregion

    #region IPrivateChannel
    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);
    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    async Task<IMessage> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetMessageAsync(id, options).ConfigureAwait(false);
        else
            return GetCachedMessage(id);
    }

    #endregion
    
    
    #region IChannel
    /// <inheritdoc />
    string IChannel.Name => $"@{Recipient}";

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(GetUser(id));
    
    #endregion
    
    /// <summary>
    ///     Returns the recipient user.
    /// </summary>
    public override string ToString() => $"@{Recipient}";
    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
    internal new SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;
}