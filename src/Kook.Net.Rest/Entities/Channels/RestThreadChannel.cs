using System.Diagnostics;
using Kook.API;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示服务器中一个基于 REST 的帖子频道，可以浏览、发布和回复帖子。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestThreadChannel : RestGuildChannel, IThreadChannel
{
    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public int PostCreationInterval { get; private set; }

    /// <inheritdoc />
    public int? ReplyInterval { get; private set; }

    /// <inheritdoc />
    public ThreadLayout? DefaultLayout { get; private set; }

    /// <inheritdoc />
    public ThreadSortOrder? DefaultSortOrder { get; private set; }

    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }

    /// <inheritdoc />
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    internal RestThreadChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id)
    {
        Type = ChannelType.Text;
        Topic = string.Empty;
    }

    internal static new RestThreadChannel Create(BaseKookClient kook, IGuild guild, Channel model)
    {
        RestThreadChannel entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    /// <inheritdoc />
    internal override void Update(Channel model)
    {
        base.Update(model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic ?? string.Empty;
        PostCreationInterval = model.SlowMode / 1000;
        if (model.SlowModeReply.HasValue)
            ReplyInterval = model.SlowModeReply.Value / 1000;
        if (model.DefaultLayout.HasValue)
            DefaultLayout = model.DefaultLayout.Value;
        if (model.SortOrder.HasValue)
            DefaultSortOrder = model.SortOrder.Value;
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual async Task ModifyAsync(Action<ModifyThreadChannelProperties> func, RequestOptions? options = null)
    {
        Model model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions? options = null) =>
        ChannelHelper.UpdateAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadCategoriesAsync(Kook.RequestOptions)" />
    public async Task<IReadOnlyCollection<IThreadCategory>> GetThreadCategoriesAsync(RequestOptions? options = null) =>
        await ThreadHelper.GetThreadCategoriesAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadAsync(System.UInt64,Kook.RequestOptions)" />
    public async Task<RestThread> GetThreadAsync(ulong id, RequestOptions? options = null) =>
        await ThreadHelper.GetThreadAsync(this, Kook, id, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(
        int limit = KookConfig.MaxThreadsPerBatch, IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, null, ThreadSortOrder.Inherited, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(System.DateTimeOffset,Kook.ThreadSortOrder,System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, referenceTimestamp, sortOrder, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(Kook.IThread,Kook.ThreadSortOrder,System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, sortOrder switch
        {
            ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            ThreadSortOrder.Inherited when DefaultSortOrder is ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.Inherited when DefaultSortOrder is ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            _ => null
        }, sortOrder, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,System.String,System.String,Kook.IThreadCategory,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, string content, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, content, cover, category, tags, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,Kook.ICard,System.String,Kook.IThreadCategory,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, ICard card, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, card, cover, category, tags, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,System.Collections.Generic.IEnumerable{Kook.ICard},System.String,Kook.IThreadCategory,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, cards, cover, category, tags, options);

    /// <inheritdoc />
    public async Task DeleteThreadAsync(ulong threadId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadAsync(this, Kook, threadId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadAsync(IThread thread, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadAsync(this, Kook, thread, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadContentAsync(ulong threadId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadContentAsync(this, Kook, threadId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadContentAsync(IThread thread, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadContentAsync(this, Kook, thread, options).ConfigureAwait(false);

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
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取所有能够查看该频道或当前在该频道中的用户。此方法会根据 <see cref="Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果有 3000 名用户，而 <see cref="Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 60 个单独请求，因此异步枚举器会异步枚举返回 60 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 60 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的服务器用户集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options);

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

    private string DebuggerDisplay => $"{Name} ({Id}, Thread)";

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

    #region IThreadChannel

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IThreadCategory>> IThreadChannel.GetThreadCategoriesAsync(RequestOptions? options) =>
        await GetThreadCategoriesAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.GetThreadAsync(ulong id, RequestOptions? options) =>
        await GetThreadAsync(id, options).ConfigureAwait(false);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(int limit,
        IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(limit, category, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder, int limit, IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(referenceTimestamp, sortOrder, limit, category, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder, int limit, IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(referenceThread, sortOrder, limit, category, options);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, string content, string? cover, IThreadCategory? category,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, content, cover, category, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, ICard card, string? cover, IThreadCategory? category,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, card, cover, category, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover, IThreadCategory? category,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, cards, cover, category, tags, options).ConfigureAwait(false);

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

    #region INestedChannel

    /// <inheritdoc />
    async Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        CategoryId.HasValue && mode == CacheMode.AllowDownload
            ? await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false) as ICategoryChannel
            : null;

    #endregion
}
