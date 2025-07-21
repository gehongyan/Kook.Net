using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Kook.API;
using Kook.API.Rest;
using Thread = Kook.API.Thread;

namespace Kook.Rest;

internal static class ThreadHelper
{
    #region Thread Categories

    public static async Task<IReadOnlyCollection<RestThreadCategory>> GetThreadCategoriesAsync(
        IThreadChannel channel, BaseKookClient client, RequestOptions? options)
    {
        GetThreadCategoriesResponse response = await client.ApiClient
            .GetThreadCategoriesAsync(channel.Id).ConfigureAwait(false);
        ThreadCategory[] models = response.List;
        IEnumerable<RestThreadCategory> entities = models
            .Select(x => RestThreadCategory.Create(client, channel, x));
        return [..entities];
    }

    #endregion

    #region Threads

    public static async Task<RestThread> GetThreadAsync(IThreadChannel channel, BaseKookClient kook,
        ulong id, RequestOptions? options)
    {
        ExtendedThread model = await kook.ApiClient.GetThreadAsync(channel.Id, id, options).ConfigureAwait(false);
        RestGuildUser author = RestGuildUser.Create(kook, channel.Guild, model.User);
        RestThread entity = RestThread.Create(kook, channel, author, model);
        return entity;
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(
        IThreadChannel channel, BaseKookClient client, DateTimeOffset? referenceTimestamp,
        ThreadSortOrder sortOrder, int limit, IThreadCategory? category, RequestOptions? options)
    {
        return new PagedAsyncEnumerable<RestThread, DateTimeOffset?>(
            KookConfig.MaxThreadsPerBatch,
            async (info, _) =>
            {
                QueryThreadsResponse response = await client.ApiClient.QueryThreadsAsync(
                        channel.Id, category?.Id, info.PageSize, sortOrder, info.Position, options)
                    .ConfigureAwait(false);
                ExtendedThread[] models = response.Items;
                IEnumerable<RestThread> entities = models.Select(x =>
                {
                    RestGuildUser author = RestGuildUser.Create(client, channel.Guild, x.User);
                    return RestThread.Create(client, channel, author, x);
                });
                return [..entities];
            },
            (info, lastPage) =>
            {
                if (lastPage.Count < info.PageSize)
                    return false;
                RestThread? lastThread = lastPage.LastOrDefault();
                info.Position = sortOrder switch
                {
                    ThreadSortOrder.LatestActivity => lastThread?.LatestActiveTimestamp,
                    ThreadSortOrder.CreationTime => lastThread?.Timestamp,
                    ThreadSortOrder.Inherited when channel.DefaultSortOrder is ThreadSortOrder.LatestActivity => lastThread?.LatestActiveTimestamp,
                    ThreadSortOrder.Inherited when channel.DefaultSortOrder is ThreadSortOrder.CreationTime => lastThread?.Timestamp,
                    _ => null
                };
                return info.Position.HasValue;
            },
            referenceTimestamp,
            limit);
    }

    public static async Task<RestThread> CreateThreadAsync(IThreadChannel channel, BaseKookClient client,
        string title, string content, bool isKMarkdown, string? cover, IThreadCategory? category, ThreadTag[]? tags,
        RequestOptions? options)
    {
        Card card = new CardBuilder(CardTheme.Invisible)
            .AddModule(new SectionModuleBuilder(content, isKMarkdown))
            .Build();
        return await CreateThreadAsync(channel, client, title, [card], cover, category, tags, options);
    }

    public static async Task<RestThread> CreateThreadAsync(IThreadChannel channel, BaseKookClient client,
        string title, ICard card, string? cover, IThreadCategory? category, ThreadTag[]? tags,
        RequestOptions? options) =>
        await CreateThreadAsync(channel, client, title, [card], cover, category, tags, options);

    public static async Task<RestThread> CreateThreadAsync(IThreadChannel channel, BaseKookClient client,
        string title, IEnumerable<ICard> cards, string? cover, IThreadCategory? category, ThreadTag[]? tags,
        RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        CreateThreadParams args = new()
        {
            ChannelId = channel.Id,
            GuildId = channel.GuildId,
            ThreadCategoryId = category?.Id,
            Title = title,
            Cover = cover,
            Content = json,
            TagIds = tags?.Select(x => x.Id).ToArray()
        };
        Thread model = await client.ApiClient.CreateThreadAsync(args, options).ConfigureAwait(false);
        RestGuildUser author = RestGuildUser.Create(client, channel.Guild, model.User);
        RestThread entity = RestThread.Create(client, channel, author, model);
        return entity;
    }

    public static async Task DeleteThreadAsync(IThreadChannel channel, BaseKookClient client,
        IThread thread, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, thread.Id, null, options).ConfigureAwait(false);

    public static async Task DeleteThreadAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, null, options).ConfigureAwait(false);

    public static async Task DeleteThreadContentAsync(IThreadChannel channel, BaseKookClient client,
        IThread thread, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, thread.Id, thread.PostId, options).ConfigureAwait(false);

    public static async Task DeleteThreadContentAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, RequestOptions? options)
    {
        RestThread thread = await GetThreadAsync(channel, client, threadId, options).ConfigureAwait(false);
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, thread.PostId, options).ConfigureAwait(false);
    }

    #endregion

    #region Thread Posts

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadPost>> GetThreadPostsAsync(IThread thread,
        BaseKookClient client, int limit, RequestOptions? options = null) =>
        GetThreadPostsAsync(thread, client, referenceTimestamp: null, SortMode.Ascending, limit, options);

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadPost>> GetThreadPostsAsync(IThread thread,
        BaseKookClient client, IThreadPost referencePost, SortMode sortMode, int limit, RequestOptions? options) =>
        GetThreadPostsAsync(thread, client, referencePost.Timestamp, sortMode, limit, options);

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadPost>> GetThreadPostsAsync(IThread thread,
        BaseKookClient client, DateTimeOffset? referenceTimestamp, SortMode sortMode, int limit, RequestOptions? options)
    {
        IAsyncEnumerable<IReadOnlyCollection<ExtendedThreadPost>> models = client.ApiClient.QueryThreadPostsAsync(
            thread.Channel.Id, thread.Id, null, limit, 1, sortMode, referenceTimestamp, options);
        return models.Select<IReadOnlyCollection<ExtendedThreadPost>, IReadOnlyCollection<RestThreadPost>>(x =>
        {
            ImmutableArray<RestThreadPost>.Builder builder = ImmutableArray.CreateBuilder<RestThreadPost>(x.Count);
            foreach (ExtendedThreadPost items in x)
            {
                RestGuildUser author = RestGuildUser.Create(client, thread.Channel.Guild, items.User);
                RestThreadPost entity = RestThreadPost.Create(client, thread, author, items);
                builder.Add(entity);
            }
            return builder.ToImmutable();
        });
    }

    public static async Task<RestThreadPost> CreateThreadPostAsync(IThread thread, BaseKookClient client,
        string content, bool isKMarkdown, RequestOptions? options)
    {
        Card card = new CardBuilder(CardTheme.Invisible)
            .AddModule(new SectionModuleBuilder(content, isKMarkdown))
            .Build();
        return await CreateThreadPostAsync(thread, client, [card], options);
    }

    public static async Task<RestThreadPost> CreateThreadPostAsync(IThread thread, BaseKookClient client,
        ICard card, RequestOptions? options) =>
        await CreateThreadPostAsync(thread, client, [card], options);

    public static async Task<RestThreadPost> CreateThreadPostAsync(IThread thread, BaseKookClient client,
        IEnumerable<ICard> cards, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        CreateThreadReplyParams args = new()
        {
            ChannelId = thread.Channel.Id,
            ThreadId = thread.Id,
            ReplyId = null,
            Content = json,
        };
        ThreadPost model = await client.ApiClient.CreateThreadReplyAsync(args, options).ConfigureAwait(false);
        IUser author = await thread.Guild.GetCurrentUserAsync(CacheMode.CacheOnly)
            ?? (IUser?)client.CurrentUser
            ?? throw new InvalidOperationException("The client does not have a current user.");
        RestThreadPost entity = RestThreadPost.Create(client, thread, author, model);
        return entity;
    }

    public static async Task DeleteThreadPostAsync(IThreadChannel channel, BaseKookClient client,
        IThreadPost post, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, post.Thread.Id, post.Id, options).ConfigureAwait(false);

    public static async Task DeleteThreadPostAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, ulong postId, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, postId, options).ConfigureAwait(false);

    #endregion

    #region Thread Replies

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadReply>> GetThreadRepliesAsync(IThreadPost post,
        BaseKookClient client, int limit, RequestOptions? options = null) =>
        GetThreadRepliesAsync(post, client, referenceTimestamp: null, SortMode.Ascending, limit, options);

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadReply>> GetThreadRepliesAsync(IThreadPost post,
        BaseKookClient client, IThreadReply referenceReply, SortMode sortMode, int limit, RequestOptions? options) =>
        GetThreadRepliesAsync(post, client, referenceReply.Timestamp, sortMode, limit, options);

    public static IAsyncEnumerable<IReadOnlyCollection<RestThreadReply>> GetThreadRepliesAsync(IThreadPost post,
        BaseKookClient client, DateTimeOffset? referenceTimestamp, SortMode sortMode, int limit, RequestOptions? options)
    {
        IAsyncEnumerable<IReadOnlyCollection<ExtendedThreadPost>> models = client.ApiClient.QueryThreadPostsAsync(
            post.Thread.Channel.Id, post.Thread.Id, post.Id, limit, 1, sortMode, referenceTimestamp, options);
        return models.Select<IReadOnlyCollection<ExtendedThreadPost>, IReadOnlyCollection<RestThreadReply>>(x =>
        {
            ImmutableArray<RestThreadReply>.Builder builder = ImmutableArray.CreateBuilder<RestThreadReply>(x.Count);
            foreach (ExtendedThreadPost items in x)
            {
                RestGuildUser author = RestGuildUser.Create(client, post.Thread.Channel.Guild, items.User);
                RestThreadReply entity = RestThreadReply.Create(client, post, author, items);
                builder.Add(entity);
            }
            return builder.ToImmutable();
        });
    }

    public static async Task<RestThreadReply> CreateThreadReplyAsync(IThreadPost post, BaseKookClient client,
        string content, bool isKMarkdown, ulong? referenceReplyId, RequestOptions? options)
    {
        Card card = new CardBuilder(CardTheme.Invisible)
            .AddModule(new SectionModuleBuilder(content, isKMarkdown))
            .Build();
        return await CreateThreadReplyAsync(post, client, [card], referenceReplyId, options);
    }

    public static async Task<RestThreadReply> CreateThreadReplyAsync(IThreadPost post, BaseKookClient client,
        ICard card, ulong? referenceReplyId, RequestOptions? options) =>
        await CreateThreadReplyAsync(post, client, [card], referenceReplyId, options);

    public static async Task<RestThreadReply> CreateThreadReplyAsync(IThreadPost post, BaseKookClient client,
        IEnumerable<ICard> cards, ulong? referenceReplyId, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        CreateThreadReplyParams args = new()
        {
            ChannelId = post.Thread.Channel.Id,
            ThreadId = post.Thread.Id,
            ReplyId = referenceReplyId ?? post.Id,
            Content = json
        };
        ThreadPost? model = await client.ApiClient.CreateThreadReplyAsync(args, options).ConfigureAwait(false);
        IUser author = await post.Thread.Guild.GetCurrentUserAsync(CacheMode.CacheOnly)
            ?? (IUser?)client.CurrentUser
            ?? throw new InvalidOperationException("The client does not have a current user.");
        RestThreadReply entity = RestThreadReply.Create(client, post, author, model);
        return entity;
    }

    public static async Task DeleteThreadReplyAsync(IThreadChannel channel, BaseKookClient client,
        IThreadReply reply, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, reply.Thread.Id, reply.Id, options).ConfigureAwait(false);

    public static async Task DeleteThreadReplyAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, ulong replyId, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, replyId, options).ConfigureAwait(false);


    #endregion

    public static async Task DeleteThreadPostReplyCoreAsync(BaseKookClient client,
        ulong channelId, ulong? threadId, ulong? postId, RequestOptions? options)
    {
        DeleteThreadPostReplyParams args = new()
        {
            ChannelId = channelId,
            ThreadId = threadId,
            PostId = postId
        };
        await client.ApiClient.DeleteThreadPostAsync(args, options).ConfigureAwait(false);
    }

}
