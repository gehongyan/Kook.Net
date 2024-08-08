using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示服务器中一个基于 REST 的具有文字聊天能力的频道，可以发送和接收消息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestTextChannel : RestGuildChannel, IRestMessageChannel, ITextChannel
{
    #region RestTextChannel

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public virtual int SlowModeInterval { get; private set; }

    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }

    /// <inheritdoc />
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    internal RestTextChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id)
    {
        Type = ChannelType.Text;
        Topic = string.Empty;
    }

    internal static new RestTextChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        RestTextChannel entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic ?? string.Empty;
        SlowModeInterval = model.SlowMode / 1000;
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual async Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions? options = null)
    {
        Model model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions? options = null) =>
        ChannelHelper.UpdateAsync(this, Kook, options);

    /// <summary>
    ///     获取此频道中的用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果为此频道中的服务器用户；如果没有找到则为 <c>null</c>。 </returns>
    public Task<RestGuildUser?> GetUserAsync(ulong id, RequestOptions? options = null) =>
        ChannelHelper.GetUserAsync(this, Guild, Kook, id, options);

    /// <summary>
    ///     获取能够查看频道或当前在此频道中的所有用户。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取所有能够查看该频道或当前在该频道中的用户。此方法会根据 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果有 3000 名用户，而 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 60 个单独请求，因此异步枚举器会异步枚举返回 60 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 60 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的服务器用户集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options);

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions? options = null) =>
        ChannelHelper.GetMessageAsync(this, Kook, id, options);

    /// <inheritdoc />
    public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <inheritdoc />
    public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        RequestOptions? options = null) =>
        ChannelHelper.GetMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <inheritdoc />
    public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        RequestOptions? options = null) =>
        ChannelHelper.GetMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    /// <inheritdoc cref="M:Kook.ITextChannel.GetPinnedMessagesAsync(Kook.RequestOptions)" />
    public virtual Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) =>
        ChannelHelper.GetPinnedMessagesAsync(this, Kook, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(System.String,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null)
    {
        string name = filename ?? Path.GetFileName(path);
        return ChannelHelper.SendFileAsync(this, Kook, path, name, type, quote, ephemeralUser, options);
    }

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(System.IO.Stream,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null) =>
        ChannelHelper.SendFileAsync(this, Kook, stream, filename, type, quote, ephemeralUser, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(Kook.FileAttachment,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendFileAsync(this, Kook, attachment, quote, ephemeralUser, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendTextAsync(System.String,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, ephemeralUser, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendCardsAsync(System.Collections.Generic.IEnumerable{Kook.ICard},Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendCardsAsync(this, Kook, cards, quote, ephemeralUser, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendCardAsync(Kook.ICard,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendCardAsync(this, Kook, card, quote, ephemeralUser, options);

    /// <summary>
    ///     获取此频道的所属分组频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此频道所属的分组频道，如果当前频道不属于任何分组频道，则为 <c>null</c>。 </returns>
    public Task<ICategoryChannel?> GetCategoryAsync(RequestOptions? options = null) =>
        ChannelHelper.GetCategoryAsync(this, Kook, options);

    /// <inheritdoc />
    public Task SyncPermissionsAsync(RequestOptions? options = null) =>
        ChannelHelper.SyncPermissionsAsync(this, Kook, options);

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        await ChannelHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null,
        RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Text)";

    #region IChannel

    /// <inheritdoc />
    async Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetUserAsync(id, options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    async Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetUserAsync(id, options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(string path, string? filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(path, filename, type, (Quote?)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(Stream stream, string filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(stream, filename, type, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(attachment, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync(string text,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendTextAsync(text, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardAsync(ICard card,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardAsync(card, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardsAsync(cards, quote, ephemeralUser, options);

    /// <inheritdoc />
    async Task<IMessage?> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetMessageAsync(id, options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit,
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(referenceMessageId, dir, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(referenceMessage, dir, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions? options) =>
        await GetPinnedMessagesAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null) =>
        ChannelHelper.DeleteMessageAsync(this, messageId, Kook, options);

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
        ChannelHelper.DeleteMessageAsync(this, message.Id, Kook, options);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId,
        Action<MessageProperties> func, RequestOptions? options = null) =>
        await ChannelHelper.ModifyMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    async Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        CategoryId.HasValue && mode == CacheMode.AllowDownload
            ? await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false) as ICategoryChannel
            : null;

    #endregion
}
