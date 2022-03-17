using System.Collections.Immutable;
using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API;
using KaiHeiLa.API.Rest;
using KaiHeiLa.Net.Converters;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based channel in a guild that can send and receive messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
{
    #region SocketTextChannel

    private readonly MessageCache _messages;
    
    /// <inheritdoc />
    public string Topic { get; set; }
    /// <inheritdoc />
    public int SlowModeInterval { get; set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     An <see cref="ICategoryChannel"/> representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    public ICategoryChannel Category
        => CategoryId.HasValue ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel : null;
    /// <inheritdoc />
    public bool IsPermissionSynced { get; set; }
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuildUser> Users
        => Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannels)).ToImmutableArray();

    internal SocketTextChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild)
        : base(kaiHeiLa, id, guild)
    {
        Type = ChannelType.Text;
        if (KaiHeiLa.MessageCacheSize > 0)
            _messages = new MessageCache(KaiHeiLa);
    }
    internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketTextChannel(guild.KaiHeiLa, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode; // some guilds haven't been patched to include this yet?
        IsPermissionSynced = model.PermissionSync == 1;
    }
    
    internal void AddMessage(SocketMessage msg)
        => _messages?.Add(msg);
    internal SocketMessage RemoveMessage(Guid id)
        => _messages?.Remove(id);
    #endregion

    #region Messages
    
    /// <inheritdoc />
    public SocketMessage GetCachedMessage(Guid id)
        => _messages?.Get(id);
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessageAsync"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions options = null)
    {
        IMessage msg = _messages?.Get(id);
        if (msg == null)
            msg = await ChannelHelper.GetMessageAsync(this, KaiHeiLa, id, options).ConfigureAwait(false);
        return msg;
    }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Text, text, options, quote: quote, ephemeralUser: ephemeralUser);
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Image, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Video, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.File, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(path, fileName, options);
    //     return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Audio, createAssetResponse.Url, options, quote: quote,
    //         ephemeralUser: ephemeralUser);
    // }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.KMarkdown, text, options, quote: quote, ephemeralUser: ephemeralUser);

    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Card, json, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(ICard card, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null) => 
        SendCardMessageAsync(new[] { card }, quote, ephemeralUser, options);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyMessageAsync(this, messageId, func, KaiHeiLa, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, messageId, KaiHeiLa, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, message.Id, KaiHeiLa, options);
    
    #endregion
    
    #region Users
    /// <inheritdoc />
    public override SocketGuildUser GetUser(ulong id)
    {
        var user = Guild.GetUser(id);
        if (user != null)
        {
            var guildPerms = Permissions.ResolveGuild(Guild, user);
            var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
            if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannels))
                return user;
        }
        return null;
    }
    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;

    #region IGuildChannel

    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(GetUser(id));

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendTextMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendTextMessageAsync(text, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendImageMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendImageMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(string path, string fileName = null,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendKMarkdownMessageAsync(text, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardMessageAsync(cards, (Quote) quote, ephemeralUser, options);
    
    /// <inheritdoc />
    async Task<IMessage> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetMessageAsync(id, options).ConfigureAwait(false);
        else
            return GetCachedMessage(id);
    }

    #endregion
}