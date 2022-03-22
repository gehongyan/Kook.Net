using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.API.Rest;
using Model = KaiHeiLa.API.UserChat;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based direct-message channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestDMChannel : RestChannel, IDMChannel, IRestPrivateChannel, IRestMessageChannel
{
    #region RestDMChannel

    public new Guid Id { get; set; }

    /// <inheritdoc />
    public Guid ChatCode
    {
        get => Id;
        set => Id = value;
    }
    
    /// <summary>
    ///     Gets the current logged-in user.
    /// </summary>
    public RestUser CurrentUser { get; }
    
    /// <summary>
    ///     Gets the recipient of the channel.
    /// </summary>
    public RestUser Recipient { get; }
    
    /// <summary>
    ///     Gets a collection that is the current logged-in user and the recipient.
    /// </summary>
    public IReadOnlyCollection<RestUser> Users => ImmutableArray.Create(CurrentUser, Recipient);
    
    internal RestDMChannel(BaseKaiHeiLaClient kaiHeiLa, Guid chatCode, ulong recipientId)
        : base(kaiHeiLa, default)
    {
        Id = chatCode;
        Recipient = new RestUser(KaiHeiLa, recipientId);
        CurrentUser = new RestUser(KaiHeiLa, kaiHeiLa.CurrentUser.Id);
    }
    internal static RestDMChannel Create(BaseKaiHeiLaClient kaiHeiLa, Model model)
    {
        var entity = new RestDMChannel(kaiHeiLa, model.Code, model.Recipient.Id);
        entity.Update(model);
        return entity;
    }
    void Update(Model model)
    {
        Recipient.Update(model.Recipient);
    }

    /// <summary>
    ///     Gets a user in this channel from the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The snowflake identifier of the user.</param>
    /// <returns>
    ///     A <see cref="RestUser"/> object that is a recipient of this channel; otherwise <c>null</c>.
    /// </returns>
    public RestUser GetUser(ulong id)
    {
        if (id == Recipient.Id)
            return Recipient;
        else if (id == KaiHeiLa.CurrentUser.Id)
            return CurrentUser;
        else
            return null;
    }

    /// <inheritdoc />
    public Task CloseAsync(RequestOptions options = null)
        => ChannelHelper.DeleteDMChannelAsync(this, KaiHeiLa, options);

    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, IQuote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Text, text, options, quote: quote);
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Image, createAssetResponse.Url, options, quote: quote);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Video, createAssetResponse.Url, options, quote: quote);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.File, createAssetResponse.Url, options, quote: quote);
    }
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
    //     return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Audio, createAssetResponse.Url, options, quote: quote);
    // }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, IQuote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.KMarkdown, text, options, quote: quote);
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, IQuote quote = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendDirectMessageAsync(this, KaiHeiLa, MessageType.Card, json, options, quote: quote);
    }

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null)
        => ChannelHelper.GetDirectMessageAsync(this, KaiHeiLa, id, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, KaiHeiLa, null, Direction.Before, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, KaiHeiLa, referenceMessageId, dir, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, KaiHeiLa, referenceMessage.Id, dir, limit, true, options);
    
    #endregion

    #region Messages

    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, messageId, KaiHeiLa, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, message.Id, KaiHeiLa, options);
    
    /// <inheritdoc />
    public Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => ChannelHelper.ModifyMessageAsync(this, messageId, func, KaiHeiLa, options);

    #endregion

    #region IDMChannel
    /// <inheritdoc />
    IUser IDMChannel.Recipient => Recipient;
    #endregion
    
    #region IRestPrivateChannel
    /// <inheritdoc />
    IReadOnlyCollection<RestUser> IRestPrivateChannel.Recipients => ImmutableArray.Create(Recipient);
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
            return null;
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(referenceMessageId, dir, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(referenceMessage, dir, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }
    #endregion
    
    /// <summary>
    ///     Gets a string that represents the Username#Discriminator of the recipient.
    /// </summary>
    /// <returns>
    ///     A string that resolves to the Recipient of this channel.
    /// </returns>
    public override string ToString() => $"@{Recipient}";
    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

    #region IChannel
    
    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(GetUser(id));
    
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();

    #endregion
}