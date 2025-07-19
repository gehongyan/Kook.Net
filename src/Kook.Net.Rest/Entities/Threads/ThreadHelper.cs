using System.Text.Json;
using Kook.API;
using Kook.API.Rest;
using Thread = Kook.API.Thread;

namespace Kook.Rest;

internal static class ThreadHelper
{
    public static async Task<IReadOnlyCollection<IThreadCategory>> GetThreadCategoriesAsync(
        RestThreadChannel channel, BaseKookClient client, RequestOptions? options)
    {
        GetThreadCategoriesResponse response = await client.ApiClient
            .GetThreadCategoriesAsync(channel.Id).ConfigureAwait(false);
        ThreadCategory[] models = response.List;
        IEnumerable<RestThreadCategory> entities = models
            .Select(x => RestThreadCategory.Create(client, channel, x));
        return [..entities];
    }

    public static async Task<RestThread> GetThreadAsync(IThreadChannel channel, BaseKookClient kook,
        ulong id, RequestOptions? options)
    {
        Thread model = await kook.ApiClient.GetThreadAsync(channel.Id, id, options).ConfigureAwait(false);
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
        string title, string content, string? cover, IThreadCategory? category, ThreadTag[]? tags,
        RequestOptions? options)
    {
        Card card = new CardBuilder(CardTheme.Invisible)
            .AddModule(new SectionModuleBuilder(content))
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

    public static async Task DeleteThreadPostAsync(IThreadChannel channel, BaseKookClient client,
        IThreadPost post, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, post.Thread.Id, post.Id, options).ConfigureAwait(false);

    public static async Task DeleteThreadPostAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, ulong postId, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, postId, options).ConfigureAwait(false);

    public static async Task DeleteThreadReplyAsync(IThreadChannel channel, BaseKookClient client,
        IThreadReply reply, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, reply.Thread.Id, reply.Id, options).ConfigureAwait(false);

    public static async Task DeleteThreadReplyAsync(IThreadChannel channel, BaseKookClient client,
        ulong threadId, ulong replyId, RequestOptions? options) =>
        await DeleteThreadPostReplyCoreAsync(client, channel.Id, threadId, replyId, options).ConfigureAwait(false);

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
