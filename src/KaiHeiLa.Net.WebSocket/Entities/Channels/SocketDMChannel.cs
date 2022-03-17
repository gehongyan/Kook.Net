using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API;
using KaiHeiLa.API.Rest;
using KaiHeiLa.Net.Converters;
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
    
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Text, text, options, quote: quote);
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Image, createAssetResponse.Url, options, quote: quote);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Video, createAssetResponse.Url, options, quote: quote);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.File, createAssetResponse.Url, options, quote: quote);
    }
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(path, fileName, options);
    //     return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Audio, createAssetResponse.Url, options, quote: quote);
    // }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.KMarkdown, text, options, quote: quote);

    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, Quote quote = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Card, json, options, quote: quote);
    }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(ICard card, Quote quote = null, RequestOptions options = null) => 
        SendCardMessageAsync(new[] { card }, quote, options);
    
    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyDirectMessageAsync(this, messageId, func, KaiHeiLa, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteDirectMessageAsync(this, messageId, KaiHeiLa, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteDirectMessageAsync(this, message.Id, KaiHeiLa, options);
    
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
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendTextMessageAsync(string text,
        IQuote quote, RequestOptions options)
        => SendTextMessageAsync(text, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendImageMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendImageMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendFileMessageAsync(path, fileName, (Quote) quote, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendAudioMessageAsync(string path, string fileName = null,
    //     IQuote quote, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, RequestOptions options)
        => SendKMarkdownMessageAsync(text, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote, RequestOptions options)
        => SendCardMessageAsync(cards, (Quote) quote, options);
    
    
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